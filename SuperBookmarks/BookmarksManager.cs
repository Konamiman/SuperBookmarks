using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        private readonly IServiceProvider serviceProvider;
        private bool deletingALineDeletesTheBookmark;

        public string SolutionPath { get; set; }

        private readonly Dictionary<string, ITextView> viewsByFilename =
            new Dictionary<string, ITextView>();

        private readonly Dictionary<ITextView, List<Bookmark>> bookmarksByView =
            new Dictionary<ITextView, List<Bookmark>>();

        //Files whose bookmarks have been retrieved from storage
        //but have not been yet open since the solution was loaded.
        //Key is filename, value is line number.
        private readonly Dictionary<string, int[]> bookmarksPendingCreation =
            new Dictionary<string, int[]>();

        public BookmarksManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        internal void InitializeAfterPackageInitialization()
        {
            var options = SuperBookmarksPackage.Instance.Options;
            deletingALineDeletesTheBookmark = options.DeletingALineDeletesTheBookmark;

            options.DeletingALineDeletesTheBookmarkChanged += (sender, args) =>
            {
                deletingALineDeletesTheBookmark = ((OptionsPage)sender).DeletingALineDeletesTheBookmark;
            };
        }

        public void RegisterTextView(string fileName, ITextView newView)
        {
            Helpers.GetTaggerFor(newView.TextBuffer); //ensure tagger exists

            if (bookmarksPendingCreation.ContainsKey(fileName))
            {
                //First time that a file having saved bookmarks is open

                var buffer = newView.TextBuffer;
                var bookmarks = new List<Bookmark>();
                foreach (var lineNumber in bookmarksPendingCreation[fileName])
                {
                    var trackingSpan = Helpers.CreateTagSpan(buffer, lineNumber);
                    bookmarks.Add(new Bookmark(trackingSpan));
                }

                viewsByFilename[fileName] = newView;
                bookmarksByView[newView] = bookmarks;
                bookmarksPendingCreation.Remove(fileName);
            }
            else if (viewsByFilename.ContainsKey(fileName))
            {
                //File with bookmarks was closed and reopen, 
                //recreate the existing bookmarks in the new view

                var oldView = viewsByFilename[fileName];
                var bookmarks = bookmarksByView[oldView];
                var currentBuffer = newView.TextBuffer;

                foreach (var bookmark in bookmarks)
                {
                    var lineNumber = bookmark.GetRow(oldView.TextBuffer);
                    var trackingSpan = Helpers.CreateTagSpan(currentBuffer, lineNumber);
                    bookmark.UpdateSpan(trackingSpan);
                }

                viewsByFilename[fileName] = newView;
                bookmarksByView.Remove(oldView);
                bookmarksByView[newView] = bookmarks;
            }
            else
            {
                //File had no previous bookmarks

                viewsByFilename[fileName] = newView;
                bookmarksByView[newView] = new List<Bookmark>();
            }

            newView.TextBuffer.Properties.AddProperty("key", newView);
            newView.TextBuffer.Changed += TextBufferChanged;
            newView.TextBuffer.Changing += TextBufferOnChanging;
        }

        private void TextBufferOnChanging(object sender, TextContentChangingEventArgs eventArgs)
        {
            var buffer = eventArgs.Before.TextBuffer;
            var view = buffer.Properties["key"] as ITextView;
            var bookmarks = bookmarksByView[view];
            foreach (var bookmark in bookmarks)
            {
                bookmark.LineNumberBeforeChanging = bookmark.GetRow(buffer);
            }
        }

        private void TextBufferChanged(object sender, TextContentChangedEventArgs eventArgs)
        {
            if (!deletingALineDeletesTheBookmark)
                return;

            bool LineIsEntirelyContainedInChange(ITextSnapshotLine line, ITextChange change)
            {
                return (
                    line.Start.Position >= change.OldPosition &&
                    line.EndIncludingLineBreak.Position <= change.OldEnd) ||
                
                //This is needed for handling the deletion of blank lines

                (change.OldText == "\r\n" && change.NewText == "" &&
                    change.OldEnd == line.EndIncludingLineBreak.Position);
            }

            if (!eventArgs.Changes.IncludesLineChanges)
                return;

            var lineDeletionChanges = eventArgs.Changes.Where(ch => ch.LineCountDelta < 0).ToArray();
            if (lineDeletionChanges.Length == 0)
                return;

            var lines = eventArgs.Before.Lines;
            var buffer = (ITextBuffer) sender;
            var view = buffer.Properties["key"] as ITextView;
            var bookmarks = bookmarksByView[view];
            foreach (var change in lineDeletionChanges)
            {
                var deletedLines = lines.Where(l => LineIsEntirelyContainedInChange(l, change));
                var deletedLineNumbers = deletedLines.Select(l => l.LineNumber + 1).ToArray();

                foreach (var deletedLineNumber in deletedLineNumbers)
                {
                    var matchingBookmark = bookmarks.SingleOrDefault(b => b.LineNumberBeforeChanging == deletedLineNumber);
                    if (matchingBookmark != null)
                    {
                        var tagger = Helpers.GetTaggerFor(buffer);
                        tagger.RemoveTagSpan(matchingBookmark.TrackingSpan);
                        bookmarksByView[view].Remove(matchingBookmark);
                    }
                }
            }            
        }

        public void SetOrRemoveBookmarkInCurrentDocument()
        {
            var currentView = Helpers.GetTextViewForActiveDocument();
            var currentBuffer = currentView.TextBuffer;
            var lineNumber = currentView.Selection.ActivePoint.Position.GetContainingLine().LineNumber + 1;

            var existingBookmark = bookmarksByView[currentView].SingleOrDefault(b => b.GetRow(currentBuffer) == lineNumber);
            if (existingBookmark == null)
            {
                //Create new bookmark

                var trackingSpan = Helpers.CreateTagSpan(currentBuffer, lineNumber);
                bookmarksByView[currentView].Add(new Bookmark(trackingSpan));
            }
            else
            {
                //Line had bookmark, remove it

                var tagger = Helpers.GetTaggerFor(currentBuffer);
                tagger.RemoveTagSpan(existingBookmark.TrackingSpan);
                bookmarksByView[currentView].Remove(existingBookmark);
            }
        }

        public void ClearAllBookmarks()
        {
            foreach (var fileName in viewsByFilename.Keys)
            {
                var view = viewsByFilename[fileName];
                foreach (var bookmark in bookmarksByView[view])
                {
                    var tagger = Helpers.GetTaggerFor(view.TextBuffer);
                    tagger.RemoveTagSpan(bookmark.TrackingSpan);
                }
            }
            bookmarksByView.Clear();
            viewsByFilename.Clear();
            bookmarksPendingCreation.Clear();
        }

        public bool HasBookmarks =>
            bookmarksByView.SelectMany(b => b.Value).Any();
    }
}
