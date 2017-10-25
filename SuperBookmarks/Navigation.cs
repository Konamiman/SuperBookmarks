using System;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.IO;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        private List<string> openDocumentPaths = new List<string>();
        private string currentDocumentFolder = null;
        private string currentTextDocumentPath = null;
        private string currentProjectFolder = null;
        private List<string> currentTextDocumentPathCollection;
        private Dictionary<BookmarkActionTarget, Func<List<string>>> filesSelectors;

        private static char[] directorySeparators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        private void InitializeNavigation()
        {
            filesSelectors =
            new Dictionary<BookmarkActionTarget, Func<List<string>>>
            {
                {BookmarkActionTarget.Document, () => currentTextDocumentPathCollection},
                {BookmarkActionTarget.OpenDocuments, GetOpenDocumentsWithBookmarks },
                {BookmarkActionTarget.Folder, () => GetDocumentsInFolder(currentDocumentFolder, false) },
                {BookmarkActionTarget.FolderAndSubfolders, () => GetDocumentsInFolder(currentDocumentFolder, true) },
                {BookmarkActionTarget.Project, () => GetDocumentsInFolder(currentProjectFolder, true) },
                {BookmarkActionTarget.Solution, GetDocumentsWithBookmarks }
            };

            currentTextDocumentPathCollection = new List<string>(1);
            currentTextDocumentPathCollection.Add(null); //must always contain 1 item
        }

        public void OnTextDocumentOpen(string path)
        {
            if (!openDocumentPaths.Contains(path))
                openDocumentPaths.Add(path);
        }

        public void OnTextDocumentClosed(string path)
        {
            if (openDocumentPaths.Contains(path))
                openDocumentPaths.Remove(path);
        }

        public void OnSolutionClosed()
        {
            openDocumentPaths.Clear();
            currentDocumentFolder = null;
            currentTextDocumentPath = null;
            currentProjectFolder = null;
        }

        public void OnCurrentDocumentChanged(string path, string projectFolder)
        {
            currentDocumentFolder = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
            currentProjectFolder = projectFolder + Path.DirectorySeparatorChar;

            if (openDocumentPaths.Contains(path))
            {
                currentTextDocumentPath = path;
            }
            else
            {
                currentTextDocumentPath = null;
            }
            currentTextDocumentPathCollection[0] = currentTextDocumentPath;
        }

        private List<string> GetOpenDocumentsWithBookmarks()
        {
            return 
                openDocumentPaths.Where(p =>
                    viewsByFilename.ContainsKey(p) ?
                        bookmarksByView[viewsByFilename[p]].Count > 0 :
                        bookmarksPendingCreation.ContainsKey(p)
                ).ToList();
        }

        private List<string> GetDocumentsWithBookmarks()
        {
            return
                viewsByFilename.Keys.Where(p => bookmarksByView[viewsByFilename[p]].Count > 0)
                .Union(bookmarksPendingCreation.Keys)
                .ToList();
        }

        private List<string> GetDocumentsInFolder(string folder, bool allowRecursive)
        {
            bool IsInFolder(string path) =>
                path.StartsWith(folder);

            var openFiles = viewsByFilename.Keys
                .Where(path => bookmarksByView[viewsByFilename[path]].Count > 0 && IsInFolder(path));
            var pendingFiles = bookmarksPendingCreation.Keys
                .Where(IsInFolder);

            var allFiles = openFiles.Union(pendingFiles);
            if (!allowRecursive)
                allFiles = allFiles
                    .Where(path => path.IndexOfAny(directorySeparators, folder.Length) == -1);

            return allFiles.ToList();
        }

        public void GoToPrevIn(BookmarkActionTarget target)
        {
            GoToPrevIn(filesSelectors[target], NoOpenFilesAreAllowedFor(target));
        }

        private bool NoOpenFilesAreAllowedFor(BookmarkActionTarget target) =>
            target == BookmarkActionTarget.Solution ||
            target == BookmarkActionTarget.Project ||
            target == BookmarkActionTarget.Folder;

        private void GoToPrevIn(Func<List<string>> getEligibleDocumentPaths, bool allowNoOpenFiles = false)
        {
            var docData = GetViewAndBookmarksForCurrentDocument(allowNoOpenFiles);
            if (docData == null && !allowNoOpenFiles)
                return;

            var targetDocuments = getEligibleDocumentPaths();

            var docIndex = -1;
            if (docData != null)
            {
                // Try to navigate within the same document; if success, we're done

                var success = GoToPrevInCurrentDocument(docData, cycle: targetDocuments.Count < 2);
                if (success)
                    return;

                // Locate the current document in the list so that we can determine "the previous one"

                docIndex = targetDocuments.IndexOf(currentTextDocumentPath);
            }
            int prevDocIndex;
            if (docIndex == 0)
                prevDocIndex = targetDocuments.Count - 1;
            else if (docIndex == -1)
                prevDocIndex = 0;
            else
                prevDocIndex = docIndex - 1;

            var prevDocPath = targetDocuments[prevDocIndex];

            if (!viewsByFilename.ContainsKey(prevDocPath) || //Doc is still in the "pending bookmarks" list...
                !openDocumentPaths.Contains(prevDocPath))    //...or doc was once open but now is closed?
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

        public void GoToNextIn(BookmarkActionTarget target)
        {
            GoToNextIn(filesSelectors[target], NoOpenFilesAreAllowedFor(target));
        }

        private void GoToNextIn(Func<List<string>> getEligibleDocumentPaths, bool allowNoOpenFiles = false)
        { 
            var docData = GetViewAndBookmarksForCurrentDocument(allowNoOpenFiles);
            if (docData == null && !allowNoOpenFiles)
                return;

            var targetDocuments = getEligibleDocumentPaths();

            var docIndex = -1;
            if (docData != null)
            {
                // Try to navigate within the same document; if success, we're done

                var success = GoToNextInCurrentDocument(docData, cycle: targetDocuments.Count < 2);
                if (success)
                    return;

                // Locate the current document in the list so that we can determine "the next one"

                docIndex = targetDocuments.IndexOf(currentTextDocumentPath);
            }
            int nextDocIndex;
            if (docIndex == targetDocuments.Count - 1)
                nextDocIndex = 0;
            else if (docIndex == -1)
                nextDocIndex = targetDocuments.Count - 1;
            else
                nextDocIndex = docIndex + 1;

            var nextDocPath = targetDocuments[nextDocIndex];

            if (!viewsByFilename.ContainsKey(nextDocPath) || //Doc is still in the "pending bookmarks" list...
                !openDocumentPaths.Contains(nextDocPath))    //...or doc was once open but now is closed?
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
