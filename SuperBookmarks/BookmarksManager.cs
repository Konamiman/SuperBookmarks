using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Konamiman.SuperBookmarks
{
    class BookmarksManager
    {
        class Bookmark
        {
            public TrackingTagSpan<BookmarkTag> TrackingSpan { get; private set; }

            public Bookmark(TrackingTagSpan<BookmarkTag> trackingSpan)
            {
                this.TrackingSpan = trackingSpan;
            }

            public void UpdateSpan(TrackingTagSpan<BookmarkTag> span)
            {
                this.TrackingSpan = span;
            }

            public int GetRow(ITextBuffer buffer)
            {
                return TrackingSpan.Span
                  .GetStartPoint(buffer.CurrentSnapshot)
                  .GetContainingLine()
                  .LineNumber + 1;
            }
        }

        private readonly IServiceProvider serviceProvider;

        public BookmarksManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private readonly Dictionary<string, ITextView> viewsByFilename =
            new Dictionary<string, ITextView>();

        private readonly Dictionary<ITextView, List<Bookmark>> bookmarksByView =
            new Dictionary<ITextView, List<Bookmark>>();

        //key is filename, value is line number
        private readonly Dictionary<string, int[]> bookmarksPendingCreation =
            new Dictionary<string, int[]>();

        public void RegisterTextView(string fileName, ITextView newView)
        {
            if (bookmarksPendingCreation.ContainsKey(fileName))
            {
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
                //Recreate the existing bookmarks in the new view
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
                viewsByFilename[fileName] = newView;
                bookmarksByView[newView] = new List<Bookmark>();
            }

            newView.TextBuffer.Properties.AddProperty("key", newView);
            newView.TextBuffer.Changed += TextBufferChanged;
        }

        private void TextBufferChanged(object sender, TextContentChangedEventArgs eventArgs)
        {
            bool LineIsEntirelyContainedInChange(ITextSnapshotLine line, ITextChange change)
            {
                return (
                    line.Start.Position >= change.OldPosition &&
                    line.EndIncludingLineBreak.Position <= change.OldEnd) ||
                
                //When a blank line is deleted, the change actually reports
                //that the line break *of the previous line* was deleted.
                //This must be handled as a special case.

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
                    var matchingBookmark = bookmarks.SingleOrDefault(b => b.GetRow(buffer) == deletedLineNumber);
                    if (matchingBookmark != null)
                    {
                        var tagger = buffer.Properties.GetOrCreateSingletonProperty(() => new SimpleTagger<BookmarkTag>(buffer));
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
                //Remove existing bookmark
                var tagger = currentBuffer.Properties.GetOrCreateSingletonProperty(() => new SimpleTagger<BookmarkTag>(currentBuffer));
                tagger.RemoveTagSpan(existingBookmark.TrackingSpan); // s => s.Span == existingBookmark.TrackingSpan.Span);
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
                    var tagger = view.TextBuffer.Properties.GetOrCreateSingletonProperty(() => new SimpleTagger<BookmarkTag>(view.TextBuffer));
                    tagger.RemoveTagSpan(bookmark.TrackingSpan);
                }
            }
            bookmarksByView.Clear();
            viewsByFilename.Clear();
            bookmarksPendingCreation.Clear();
        }

        public bool HasBookmarks =>
            bookmarksByView.SelectMany(b => b.Value).Any();

        public PersistableBookmarksInfo GetPersistableInfo()
        {
            var info = new PersistableBookmarksInfo();
            foreach (var fileName in viewsByFilename.Keys)
            {
                var view = viewsByFilename[fileName];
                if (bookmarksByView[view].Count == 0) continue;
                var persistableBookmarks = new List<PersistableBookmarksInfo.Bookmark>();
                foreach (var bookmark in bookmarksByView[view])
                {
                    persistableBookmarks.Add(new PersistableBookmarksInfo.Bookmark
                    {
                        LineNumber = bookmark.GetRow(view.TextBuffer)
                    });
                }
                info.BookmarksByFilename[fileName] = persistableBookmarks.ToArray();
            }
            foreach (var filename in bookmarksPendingCreation.Keys)
            {
                var persistableBookmarks = 
                    bookmarksPendingCreation[filename]
                    .Select(l => new PersistableBookmarksInfo.Bookmark {LineNumber = l});
                info.BookmarksByFilename[filename] = persistableBookmarks.ToArray();
            }
            return info;
        }

        public void RecreateBookmarksFromPersistableInfo(PersistableBookmarksInfo info)
        {
            ClearAllBookmarks();
            bookmarksPendingCreation.Clear();

            foreach (var fileName in info.BookmarksByFilename.Keys)
            {
                var lineNumbers = info.BookmarksByFilename[fileName].Select(b => b.LineNumber).ToArray();
                bookmarksPendingCreation[fileName] = lineNumbers;
            }
        }
    }
}
