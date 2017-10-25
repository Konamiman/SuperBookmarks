using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        private readonly IServiceProvider serviceProvider;
        private bool deletingALineDeletesTheBookmark;
        private IVsEditorAdaptersFactoryService editorAdaptersFactoryService;
        private IVsTextManager textManager;

        public string SolutionPath { get; set; }

        public readonly Dictionary<string, ITextView> viewsByFilename =
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

            InitializeNavigation();
        }

        internal void InitializeAfterPackageInitialization(
            IVsEditorAdaptersFactoryService editorAdaptersFactoryService,
            IVsTextManager textManager)
        {
            this.editorAdaptersFactoryService = editorAdaptersFactoryService;
            this.textManager = textManager;

            var options = SuperBookmarksPackage.Instance.Options;
            deletingALineDeletesTheBookmark = options.DeletingALineDeletesTheBookmark;

            options.DeletingALineDeletesTheBookmarkChanged += (sender, args) =>
            {
                deletingALineDeletesTheBookmark = ((OptionsPage)sender).DeletingALineDeletesTheBookmark;
            };

            options.GlyphColorChanged += (sender, args) =>
            {
                BookmarkGlyphFactory.SetGlyphColor(options.GlyphColor);

                //Force redraw of currently visible bookmarks
                //(there must be a better way to do this...)
                var frame = SuperBookmarksPackage.Instance.CurrentWindowFrame;
                if (frame?.IsVisible() == VSConstants.S_OK)
                {
                    frame.Hide();
                    frame.Show();
                }
            };
        }

        public void RegisterTextView(string fileName, ITextView newView)
        {
            var currentBuffer = Helpers.GetRootTextBuffer(newView.TextBuffer);
            Helpers.GetTaggerFor(currentBuffer); //ensure tagger exists

            if (bookmarksPendingCreation.ContainsKey(fileName))
            {
                //First time that a file having saved bookmarks is open

                var bookmarks = new List<Bookmark>();
                foreach (var lineNumber in bookmarksPendingCreation[fileName])
                {
                    var trackingSpan = Helpers.CreateTagSpan(currentBuffer, lineNumber);
                    if(trackingSpan != null)
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

                foreach (var bookmark in bookmarks)
                {
                    var lineNumber = bookmark.GetRow(Helpers.GetRootTextBuffer(oldView.TextBuffer));
                    var trackingSpan = Helpers.CreateTagSpan(currentBuffer, lineNumber);
                    if (trackingSpan == null)
                        bookmarksByView[oldView].Remove(bookmark);
                    else
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

            if (currentBuffer.Properties.ContainsProperty("key"))
            {
                currentBuffer.Properties["key"] = newView;
            }
            else
            {
                currentBuffer.Properties.AddProperty("key", newView);
                currentBuffer.Changed += TextBufferChanged;
                currentBuffer.Changing += TextBufferOnChanging;
            }
        }

        private void TextBufferOnChanging(object sender, TextContentChangingEventArgs eventArgs)
        {
            var buffer = Helpers.GetRootTextBuffer(eventArgs.Before.TextBuffer);
            var view = buffer.Properties["key"] as ITextView;
            var bookmarks = bookmarksByView[view];
            foreach (var bookmark in bookmarks)
            {
                bookmark.LineNumberBeforeChanging = bookmark.GetRow(buffer);
            }
        }

        private void TextBufferChanged(object sender, TextContentChangedEventArgs eventArgs)
        {
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
            
            var lines = eventArgs.Before.Lines.ToArray();
            var buffer = (ITextBuffer) sender;
            var view = buffer.Properties["key"] as ITextView;
            var bookmarks = bookmarksByView[view];

            if (!deletingALineDeletesTheBookmark)
            {
                // If "deleting a line deletes the bookmark" is off, we will end up
                // with duplicate bookmakrs when lines are deleted
                // (e.g. you have bookmarks in lines 3 and 4, you delete line 3,
                // now you have two bookmarks for line 3).
                var duplicateBookmarkGroup = bookmarks.GroupBy(b => b.GetRow(buffer)).Where(g => g.Count() > 1);
                foreach (var group in duplicateBookmarkGroup)
                {
                    var bookmarksInGroup = group.ToArray();
                    for (int i = 1; i < bookmarksInGroup.Length; i++)
                        bookmarks.Remove(bookmarksInGroup[i]);
                }
                return;
            }

            foreach (var change in lineDeletionChanges)
            {
                var deletedLines = lines.Where(l => LineIsEntirelyContainedInChange(l, change));
                var deletedLineNumbers = deletedLines.Select(l => l.LineNumber + 1).ToArray();

                foreach (var deletedLineNumber in deletedLineNumbers)
                {
                    var matchingBookmark = bookmarks.SingleOrDefault(b => b.LineNumberBeforeChanging == deletedLineNumber);
                    if (matchingBookmark == null)
                        continue;

                    var tagger = Helpers.GetTaggerFor(buffer);
                    tagger.RemoveTagSpan(matchingBookmark.TrackingSpan);
                    bookmarksByView[view].Remove(matchingBookmark);
                }
            }            
        }

        class CurrentDocumentData
        {
            public ITextView TextView { get; set; }
            public ITextBuffer TextBuffer { get; set; }
            public List<Bookmark> Bookmarks { get; set; }
            public int CurrentLineNumber { get; set; }
        }

        CurrentDocumentData GetViewAndBookmarksForCurrentDocument(bool allowNoOpenFiles = false)
        {
            ITextView currentView = openDocumentPaths.Count == 0 ? null : Helpers.GetTextViewForActiveDocument();
            if (currentView == null)
            {
                if(!allowNoOpenFiles)
                    Helpers.ShowErrorMessage("I couldn't get a text view for the active document.");
                return null;
            }

            ITextBuffer currentBuffer = Helpers.GetRootTextBuffer(currentView.TextBuffer);

            if (!bookmarksByView.ContainsKey(currentView))
            {
                Helpers.ShowErrorMessage("I don't have a TextView registered for the active document.");
                return null;
            }

            var lineNumber = currentView.Selection.ActivePoint.Position.GetContainingLine().LineNumber + 1;

            return new CurrentDocumentData
            {
                TextView = currentView,
                TextBuffer = currentBuffer,
                Bookmarks = bookmarksByView[currentView],
                CurrentLineNumber = lineNumber
            };
        }

        public void SetOrRemoveBookmarkInCurrentDocument()
        {
            var docData = GetViewAndBookmarksForCurrentDocument();
            if (docData == null)
                return;

            var lineNumber = docData.TextView.Selection.ActivePoint.Position.GetContainingLine().LineNumber + 1;

            var existingBookmarks = docData.Bookmarks.Where(b => b.GetRow(docData.TextBuffer) == lineNumber).ToArray();
            if (existingBookmarks.Length == 0)
            {
                //Create new bookmark

                var trackingSpan = Helpers.CreateTagSpan(docData.TextBuffer, lineNumber);
                if (trackingSpan != null)
                {
                    docData.Bookmarks.Add(new Bookmark(trackingSpan));
                    if (docData.Bookmarks.Count == 1)
                    {
                        var path = viewsByFilename.Keys.Single(k => viewsByFilename[k] == docData.TextView);
                        SolutionExplorerFilter.OnFileGotItsFirstBookmark(path);
                    }
                }
            }
            else
            {
                foreach (var existingBookmark in existingBookmarks)
                {
                    //Line had bookmark, remove it
                    //(we can have multiple entries if "deleting line deletes bookmark" option is off)

                    var tagger = Helpers.GetTaggerFor(docData.TextBuffer);
                    tagger.RemoveTagSpan(existingBookmark.TrackingSpan);
                    docData.Bookmarks.Remove(existingBookmark);

                    if(docData.Bookmarks.Count == 0)
                    {
                        var path = viewsByFilename.Keys.Single(k => viewsByFilename[k] == docData.TextView);
                        SolutionExplorerFilter.OnFileLostItsLastBookmark(path);
                    }
                }
            }
        }

        public void OnFileDeleted(string filePath)
        {
            bookmarksPendingCreation.Remove(filePath);
            if (viewsByFilename.ContainsKey(filePath))
            {
                bookmarksByView.Remove(viewsByFilename[filePath]);
                viewsByFilename.Remove(filePath);
            }
        }

        public void OnFileRenamed(string oldPath, string newPath)
        {
            if (viewsByFilename.ContainsKey(oldPath))
            {
                viewsByFilename[newPath] = viewsByFilename[oldPath];
                viewsByFilename.Remove(oldPath);
            }

            if (bookmarksPendingCreation.ContainsKey(oldPath))
            {
                bookmarksPendingCreation[newPath] = bookmarksPendingCreation[oldPath];
                bookmarksPendingCreation.Remove(newPath);
            }
        }

        public bool HasBookmarks(string path)
        {
            return (viewsByFilename.ContainsKey(path) && bookmarksByView[viewsByFilename[path]].Any())
                   || bookmarksPendingCreation.ContainsKey(path);
        }
    }
}
