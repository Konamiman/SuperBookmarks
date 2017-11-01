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

        //The currently active (or the last active) view for each file.
        //A file will have more than one registered view if the window is split vertically.
        public readonly Dictionary<string, ITextView> activeViewsByFilename =
            new Dictionary<string, ITextView>();

        //All the registered views for each file (includes the active view as well).
        public readonly Dictionary<string, List<ITextView>> allViewsByFilename =
            new Dictionary<string, List<ITextView>>();

        //Bookmarks lists for each view. There's only one list for each file, but
        //there's one dictionary entry for each registered view
        //(so files with multiple registered views will have multiple keys
        //pointing to the same value).
        private readonly Dictionary<ITextView, List<Bookmark>> bookmarksByView =
            new Dictionary<ITextView, List<Bookmark>>();

        //Files that have bookmarks but aren't currently open.
        //Key is filename, value is line numbers.
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
                deletingALineDeletesTheBookmark = ((GeneralOptionsPage)sender).DeletingALineDeletesTheBookmark;
            };
        }

        public void RegisterTextView(string fileName, ITextView newView)
        {
            void RegisterInAllViewsList(ITextView view)
            {
                if (!allViewsByFilename.ContainsKey(fileName))
                    allViewsByFilename.Add(fileName, new List<ITextView>());

                allViewsByFilename[fileName].Add(view);

                //Yep, we want to reference the same list, that's on purpose.
                bookmarksByView[view] = bookmarksByView[activeViewsByFilename[fileName]];

                view.Closed += OnViewClosed;
                view.GotAggregateFocus += OnViewGotFocus;
                view.LostAggregateFocus += OnViewLostFocus;
            }

            var currentBuffer = Helpers.GetRootTextBuffer(newView.TextBuffer);

            if (bookmarksPendingCreation.ContainsKey(fileName))
            {
                //File has bookmarks

                var bookmarks = new List<Bookmark>();
                foreach (var lineNumber in bookmarksPendingCreation[fileName])
                    RegisterAndCreateBookmark(bookmarks, currentBuffer, fileName, lineNumber);

                activeViewsByFilename[fileName] = newView;
                bookmarksByView[newView] = bookmarks;
                bookmarksPendingCreation.Remove(fileName);
            }
            else if(!activeViewsByFilename.ContainsKey(fileName))
            {
                //File had no previous bookmarks

                activeViewsByFilename[fileName] = newView;
                bookmarksByView[newView] = new List<Bookmark>();
            }

            RegisterInAllViewsList(newView);
            Helpers.Debug($"Register text view {newView.GetHashCode()} for {fileName}, total: {allViewsByFilename[fileName].Count}");

            newView.Properties.AddProperty("fileName", fileName);
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

        private void UnregisterTextViews(string fileName)
        {
            Helpers.Debug("UNregister text views request: " + fileName);

            var view = activeViewsByFilename[fileName];
            var bookmarks = bookmarksByView[view];
            var buffer = view.TextBuffer;
            var lineNumbers = bookmarks.Select(b => b.GetRow(buffer)).ToArray();

            if (lineNumbers.Length > 0)
            {
                foreach (var bookmark in bookmarks.ToArray())
                    UnregisterAndDeleteBookmark(bookmarks, bookmark, buffer, notifyIfLastBookmark: false);

                bookmarksPendingCreation[fileName] = lineNumbers;
            }

            foreach(var registeredView in allViewsByFilename[fileName])
            {
                registeredView.Closed -= OnViewClosed;
                registeredView.GotAggregateFocus -= OnViewGotFocus;
                registeredView.LostAggregateFocus -= OnViewLostFocus;
            }

            activeViewsByFilename.Remove(fileName);
            allViewsByFilename.Remove(fileName);
            bookmarksByView.Remove(view);
        }

        private string GetFilenameOfView(ITextView view)
        {
            if (!view.Properties.TryGetProperty<string>("fileName", out string fileName))
            {
                Helpers.LogError("I couldn't get the filename for an ITextView");
                return null;
            }

            return fileName;
        }

        private void OnViewGotFocus(object sender, EventArgs e)
        {
            var view = (ITextView)sender;
            var fileName = GetFilenameOfView(view);
            if (fileName == null) return;

            Helpers.Debug($"View got focus: {view.GetHashCode()} for {fileName}");

            activeViewsByFilename[fileName] = view;
        }

        private void OnViewLostFocus(object sender, EventArgs e)
        {
            var view = (ITextView)sender;
            var fileName = GetFilenameOfView(view);
            if (fileName == null) return;

            Helpers.Debug($"View lost focus: {view.GetHashCode()} for {fileName}");
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            var view = (ITextView)sender;
            var fileName = GetFilenameOfView(view);
            if (fileName == null) return;

            allViewsByFilename[fileName].Remove(view);

            Helpers.Debug($"View closed: {view.GetHashCode()} for {fileName}, remaning: {allViewsByFilename[fileName].Count}");
        }

        public void SetOrRemoveBookmarkInCurrentDocument()
        {
            if (!activeViewsByFilename.ContainsKey(currentTextDocumentPath))
                return;

            var view = activeViewsByFilename[currentTextDocumentPath];
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
