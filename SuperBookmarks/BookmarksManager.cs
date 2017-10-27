using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Tagging;

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

        //Files that have bookmarks but aren't currently open.
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

            if (bookmarksPendingCreation.ContainsKey(fileName))
            {
                //File has bookmarks

                var bookmarks = new List<Bookmark>();
                foreach (var lineNumber in bookmarksPendingCreation[fileName])
                    RegisterAndCreateBookmark(bookmarks, currentBuffer, fileName, lineNumber);

                viewsByFilename[fileName] = newView;
                bookmarksByView[newView] = bookmarks;
                bookmarksPendingCreation.Remove(fileName);
            }
            else
            {
                //File had no previous bookmarks

                viewsByFilename[fileName] = newView;
                bookmarksByView[newView] = new List<Bookmark>();
            }

            if (currentBuffer.Properties.ContainsProperty("view"))
            {
                currentBuffer.Properties["view"] = newView;
            }
            else
            {
                currentBuffer.Properties.AddProperty("view", newView);
                currentBuffer.Changed += TextBufferChanged;
                currentBuffer.Changing += TextBufferOnChanging;
            }

            currentBuffer.Properties["fileName"] = fileName;
        }

        private void UnregisterTextView(string fileName)
        {
            var view = viewsByFilename[fileName];
            var bookmarks = bookmarksByView[view];
            var buffer = view.TextBuffer;
            var lineNumbers = bookmarks.Select(b => b.GetRow(buffer)).ToArray();

            if (lineNumbers.Length > 0)
            {
                foreach (var bookmark in bookmarks.ToArray())
                    UnregisterAndDeleteBookmark(bookmarks, bookmark, buffer, notifyIfLastBookmark: false);

                bookmarksPendingCreation[fileName] = lineNumbers;
            }

            viewsByFilename.Remove(fileName);
            bookmarksByView.Remove(view);
        }

        public void SetOrRemoveBookmarkInCurrentDocument()
        {
            if (!viewsByFilename.ContainsKey(currentTextDocumentPath))
                return;

            var view = viewsByFilename[currentTextDocumentPath];
            var bookmarks = bookmarksByView[view];
            var buffer = view.TextBuffer;

            var lineNumber = GetCurrentLineNumber(view);

            var existingBookmarks = bookmarks.Where(b => b.GetRow(buffer) == lineNumber).ToArray();
            if (existingBookmarks.Length == 0)
            {
                //Bookmarks didn't exist, create it

                RegisterAndCreateBookmark(bookmarks, buffer, currentTextDocumentPath, lineNumber);
            }
            else
            {
                foreach (var existingBookmark in existingBookmarks)
                {
                    //Line had bookmark, remove it
                    //(we can have multiple entries if "deleting line deletes bookmark" option is off)

                    UnregisterAndDeleteBookmark(bookmarks, existingBookmark, buffer);
                }
            }
        }

        private void RegisterAndCreateBookmark(List<Bookmark> bookmarks, ITextBuffer buffer, string fileName, int lineNumber)
        {
            var trackingSpan = CreateTagSpan(buffer, lineNumber);
            if (trackingSpan != null)
            {
                bookmarks.Add(new Bookmark(trackingSpan));
                if (bookmarks.Count == 1)
                    SolutionExplorerFilter.OnFileGotItsFirstBookmark(fileName);
            }
        }

        private TrackingTagSpan<BookmarkTag> CreateTagSpan(ITextBuffer buffer, int lineNumber)
        {
            var snapshot = buffer.CurrentSnapshot;

            //This can happen if the file is edited outside Visual Studio
            //while the solution is closed
            if (lineNumber > snapshot.LineCount)
                return null;

            var line = snapshot.GetLineFromLineNumber(lineNumber - 1);
            var span = snapshot.CreateTrackingSpan(new SnapshotSpan(line.Start, 0), SpanTrackingMode.EdgeExclusive);
            var tagger = Helpers.GetTaggerFor(buffer);
            var trackingSpan = tagger.CreateTagSpan(span, new BookmarkTag());
            return trackingSpan;
        }

        private void UnregisterAndDeleteBookmark(List<Bookmark> bookmarks, Bookmark bookmark, ITextBuffer buffer, bool notifyIfLastBookmark = true)
        {
            var tagger = Helpers.GetTaggerFor(buffer);
            tagger.RemoveTagSpan(bookmark.TrackingSpan);
            bookmarks.Remove(bookmark);

            if (bookmarks.Count == 0 && notifyIfLastBookmark)
                SolutionExplorerFilter.OnFileLostItsLastBookmark(buffer.Properties["fileName"] as string);
        }
    }
}
