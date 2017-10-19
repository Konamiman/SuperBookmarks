using System;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        private Dictionary<string, IVsWindowFrame> openDocumentPathsAndFrames =
            new Dictionary<string, IVsWindowFrame>();
        private string currentDocumentFolder = null;
        private string currentTextDocumentPath = null;
        private int currentDocumentFolderLength = 0;
        private IWpfTextView currentDocumentView = null;
        private bool folderNavigationIsRecursive = false;

        public void OnTextDocumentOpen(string path, IVsWindowFrame windowFrame)
        {
            if (!openDocumentPathsAndFrames.ContainsKey(path))
                openDocumentPathsAndFrames.Add(path, windowFrame);
        }

        public void OnTextDocumentClosed(string path)
        {
            if (openDocumentPathsAndFrames.ContainsKey(path))
                openDocumentPathsAndFrames.Remove(path);
        }

        public void OnSolutionClosed()
        {
            openDocumentPathsAndFrames.Clear();
            currentDocumentFolder = null;
            currentTextDocumentPath = null;
        }

        public void OnCurrentDocumentChanged(string path)
        {
            currentDocumentFolder = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
            currentDocumentFolderLength = currentDocumentFolder.Length;

            if (openDocumentPathsAndFrames.ContainsKey(path))
            {
                currentTextDocumentPath = path;
                var vsTextView = VsShellUtilities.GetTextView(openDocumentPathsAndFrames[path]);
                currentDocumentView = editorAdaptersFactoryService.GetWpfTextView(vsTextView);
            }
            else
            {
                currentTextDocumentPath = null;
                currentDocumentView = null;
            }
        }

        public void GoToPrevInCurrentDocument()
        {
            var docData = GetViewAndBookmarksForCurrentDocument();
            if (docData == null)
                return;

            GoToPrevInCurrentDocument(docData, cycle: true);
        }

        //Returns false when no cycling is allowed and there are no more bookmarks
        //from the current point to the start of the document
        private bool GoToPrevInCurrentDocument(CurrentDocumentData docData, bool cycle)
        {
            if (docData.Bookmarks.Count == 0)
                return true;

            var lineNumbers = docData.Bookmarks.Select(b => b.GetRow(docData.TextBuffer)).OrderByDescending(x => x).ToList();
            var prevLine = lineNumbers.FirstOrDefault(l => l < docData.CurrentLineNumber);

            if (prevLine == 0 && !cycle)
                return false;

            var targetLineNumber =
                prevLine == 0
                    ? lineNumbers.First()
                    : prevLine;

            MoveToLineNumber(docData.TextView, targetLineNumber);
            return true;
        }

        private List<string> GetOpenDocumentsWithBookmarks()
        {
            return 
                openDocumentPathsAndFrames.Keys.Where(p =>
                    viewsByFilename.ContainsKey(p) ?
                        bookmarksByView[viewsByFilename[p]].Count > 0 :
                        bookmarksPendingCreation.ContainsKey(p)
                ).ToList();
        }

        private static char[] directorySeparators = new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};

        private List<string> GetOpenDocumentsInCurrentFolder()
        {
            bool IsInCurrentFolder(string path) =>
                path.StartsWith(currentDocumentFolder);

            var openFiles = viewsByFilename.Keys
                .Where(path => bookmarksByView[viewsByFilename[path]].Count > 0 && IsInCurrentFolder(path));
            var pendingFiles = bookmarksPendingCreation.Keys
                .Where(IsInCurrentFolder);

            var allFiles = openFiles.Union(pendingFiles);
            if (!folderNavigationIsRecursive)
                allFiles = allFiles
                    .Where(path => path.IndexOfAny(directorySeparators, currentDocumentFolderLength) == -1);

            return allFiles.ToList();
        }

        public void GoToPrevInOpenFiles()
        {
            GoToPrevIn(GetOpenDocumentsWithBookmarks);
        }

        public void GoToPrevInFolder()
        {
            GoToPrevIn(GetOpenDocumentsInCurrentFolder);
        }

        public void GoToPrevIn(Func<List<string>> getEligibleDocumentPaths)
        {
            var docData = GetViewAndBookmarksForCurrentDocument();
            if (docData == null)
                return;

            var openDocumentsWithBookmarks = getEligibleDocumentPaths();

            // Try to navigate within the same document; if success, we're done

            var success = GoToPrevInCurrentDocument(docData, cycle: openDocumentsWithBookmarks.Count < 2);
            if (success)
                return;

            // Ok, which one is "the previous document"?

            var docIndex = openDocumentsWithBookmarks.IndexOf(currentTextDocumentPath);
            int prevDocIndex;
            if (docIndex == 0)
                prevDocIndex = openDocumentsWithBookmarks.Count - 1;
            else if (docIndex == -1)
                prevDocIndex = 0;
            else
                prevDocIndex = docIndex - 1;

            var prevDocPath = openDocumentsWithBookmarks[prevDocIndex];

            // The document could never have been displayed since the solution was open
            // (that is, it's still in the "pending bookmarks" list)
            // if it was open when the solution was closed the last time

            if (!viewsByFilename.ContainsKey(prevDocPath))
            {
                VsShellUtilities.OpenDocument(this.serviceProvider,
                    prevDocPath,
                    VSConstants.LOGVIEWID_TextView,
                    out var dummyHierarchy, out var dummyId,
                    out var dummyWindowFrame,
                    out var dummyVsTextView);
            }

            // Convert stuff around and finally, navigate to the desired line!

            var prevDocView = viewsByFilename[prevDocPath];
            var prevDocBuffer = Helpers.GetRootTextBuffer(prevDocView.TextBuffer);
            var lastBookmarkLine = bookmarksByView[prevDocView].Select(b => b.GetRow(prevDocBuffer)).Max();
            var vsBuffer = editorAdaptersFactoryService.GetBufferAdapter(prevDocBuffer);

            textManager.NavigateToLineAndColumn(
                vsBuffer,
                VSConstants.LOGVIEWID_TextView,
                lastBookmarkLine - 1, 0, lastBookmarkLine - 1, 0);
        }

        public void GoToNextInCurrentDocument()
        {
            var docData = GetViewAndBookmarksForCurrentDocument();
            if (docData == null)
                return;

            GoToNextInCurrentDocument(docData, cycle: true);
        }

        //Returns false when no cycling is allowed and there are no more bookmarks
        //from the current point to the end of the document
        private bool GoToNextInCurrentDocument(CurrentDocumentData docData, bool cycle)
        {
            if (docData.Bookmarks.Count == 0)
                return true;

            var lineNumbers = docData.Bookmarks.Select(b => b.GetRow(docData.TextBuffer)).OrderBy(x => x).ToList();
            var nextLine = lineNumbers.FirstOrDefault(l => l > docData.CurrentLineNumber);

            if (nextLine == 0 && !cycle)
                return false;

            var targetLineNumber =
                nextLine == 0
                    ? lineNumbers.First()
                    : nextLine;

            MoveToLineNumber(docData.TextView, targetLineNumber);
            return true;
        }

        public void GoToNextInOpenFiles()
        {
            GoToNextIn(GetOpenDocumentsWithBookmarks);
        }

        public void GoToNextInFolder()
        {
            GoToNextIn(GetOpenDocumentsInCurrentFolder);
        }

        public void GoToNextIn(Func<List<string>> getEligibleDocumentPaths)
        { 
            var docData = GetViewAndBookmarksForCurrentDocument();
            if (docData == null)
                return;

            var openDocumentsWithBookmarks = getEligibleDocumentPaths();

            // Try to navigate within the same document; if success, we're done

            var success = GoToNextInCurrentDocument(docData, cycle: openDocumentsWithBookmarks.Count < 2);
            if (success)
                return;

            // Ok, which one is "the next document"?

            var docIndex = openDocumentsWithBookmarks.IndexOf(currentTextDocumentPath);
            int nextDocIndex;
            if (docIndex == openDocumentsWithBookmarks.Count - 1)
                nextDocIndex = 0;
            else if (docIndex == -1)
                nextDocIndex = openDocumentsWithBookmarks.Count - 1;
            else
                nextDocIndex = docIndex + 1;

            var nextDocPath = openDocumentsWithBookmarks[nextDocIndex];

            // The document could never have been displayed since the solution was open
            // (that is, it's still in the "pending bookmarks" list)
            // if it was open when the solution was closed the last time

            if (!viewsByFilename.ContainsKey(nextDocPath))
            {
                VsShellUtilities.OpenDocument(this.serviceProvider,
                    nextDocPath,
                    VSConstants.LOGVIEWID_TextView,
                    out var dummyHierarchy, out var dummyId,
                    out var dummyWindowFrame,
                    out var dummyVsTextView);
            }

            // Convert stuff around and finally, navigate to the desired line!

            var nextDocView = viewsByFilename[nextDocPath];
            var nextDocBuffer = Helpers.GetRootTextBuffer(nextDocView.TextBuffer);
            var lastBookmarkLine = bookmarksByView[nextDocView].Select(b => b.GetRow(nextDocBuffer)).Min();
            var vsBuffer = editorAdaptersFactoryService.GetBufferAdapter(nextDocBuffer);

            textManager.NavigateToLineAndColumn(
                vsBuffer,
                VSConstants.LOGVIEWID_TextView,
                lastBookmarkLine - 1, 0, lastBookmarkLine - 1, 0);
        }

        private void MoveToLineNumber(ITextView textView, int lineNumber)
        {
            if (!SuperBookmarksPackage.Instance.ActiveDocumentIsText)
            {
                // If a non-text document is currently open,
                // make the text document visible

                var buffer = Helpers.GetRootTextBuffer(textView.TextBuffer);
                var vsBuffer = editorAdaptersFactoryService.GetBufferAdapter(buffer);

                textManager.NavigateToLineAndColumn(
                    vsBuffer,
                    VSConstants.LOGVIEWID_TextView,
                    lineNumber - 1, 0, lineNumber - 1, 0);

                return;
            }

            var point = textView.TextSnapshot.GetLineFromLineNumber(lineNumber - 1).Start;
            textView.Caret.MoveTo(point);
            if (textView is IWpfTextView)
            {
                var height = ((IWpfTextView)textView).VisualElement.ActualHeight;
                textView.DisplayTextLineContainingBufferPosition(point, height / 2, ViewRelativePosition.Top);
            }
            else
            {
                textView.Caret.EnsureVisible();
            }
        }
    }

}
