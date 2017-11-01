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

            currentTextDocumentPathCollection = new List<string>(1) { null }; //must always contain 1 item
        }

        private List<string> GetOpenDocumentsWithBookmarks()
        {
            return 
                openDocumentPaths.Where(p =>
                    activeViewsByFilename.ContainsKey(p) ?
                        bookmarksByView[activeViewsByFilename[p]].Count > 0 :
                        bookmarksPendingCreation.ContainsKey(p)
                ).ToList();
        }

        private List<string> GetDocumentsWithBookmarks()
        {
            return
                activeViewsByFilename.Keys.Where(p => bookmarksByView[activeViewsByFilename[p]].Count > 0)
                .Union(bookmarksPendingCreation.Keys)
                .ToList();
        }

        private List<string> GetDocumentsInFolder(string folder, bool allowRecursive)
        {
            bool IsInFolder(string path) =>
                path.StartsWith(folder);

            var openFiles = activeViewsByFilename.Keys
                .Where(path => bookmarksByView[activeViewsByFilename[path]].Count > 0 && IsInFolder(path));
            var pendingFiles = bookmarksPendingCreation.Keys
                .Where(IsInFolder);

            var allFiles = openFiles.Union(pendingFiles);
            if (!allowRecursive)
                allFiles = allFiles
                    .Where(path => path.IndexOfAny(directorySeparators, folder.Length) == -1);

            return allFiles.ToList();
        }

        public void GoToPrevIn(BookmarkActionTarget target) =>
            GoToPrevIn(filesSelectors[target]);

        private void GoToPrevIn(Func<List<string>> getEligibleDocumentPaths)
        {
            var targetDocuments = getEligibleDocumentPaths();

            var currentDocIndex = -1;
            if (currentTextDocumentPath != null &&
                (currentDocIndex = targetDocuments.IndexOf(currentTextDocumentPath)) != -1)
            {
                // Try to navigate within the currently open document; if success, we're done

                var success = GoToPrevInDocument(currentTextDocumentPath, cycle: targetDocuments.Count < 2);
                if (success)
                    return;
            }

            if(!targetDocuments.Any())
            {
                Helpers.LogError("I have been asked to navigate to the previous bookmark in another file, but the list of target files I got is empty. Try closing and reopening the affected file(s).");
                return;
            }
            if (targetDocuments.Contains(null))
            {
                Helpers.LogError("I have been asked to navigate to the previous bookmark in another file, but the list of target files contains \"null\".");
                return;
            }

            int prevDocIndex;
            if (currentDocIndex == 0)
                prevDocIndex = targetDocuments.Count - 1;
            else if (currentDocIndex == -1)
                prevDocIndex = 0;
            else
                prevDocIndex = currentDocIndex - 1;

            var prevDocPath = targetDocuments[prevDocIndex];

            if (!activeViewsByFilename.ContainsKey(prevDocPath) || //Doc is still in the "pending bookmarks" list...
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

            var prevDocView = activeViewsByFilename[prevDocPath];
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
        private bool GoToPrevInDocument(string fileName, bool cycle)
        {
            var view = activeViewsByFilename[fileName];
            var buffer = view.TextBuffer;
            var bookmarks = bookmarksByView[view];
            
            if (bookmarks.Count == 0)
                return true;

            var currentLineNumber = GetCurrentLineNumber(view);
            var lineNumbers = bookmarks.Select(b => b.GetRow(buffer)).OrderByDescending(x => x).ToList();
            var prevLine = lineNumbers.FirstOrDefault(l => l < currentLineNumber);

            if (prevLine == 0 && !cycle)
                return false;

            var targetLineNumber =
                prevLine == 0
                    ? lineNumbers.First()
                    : prevLine;

            MoveToLineNumber(view, targetLineNumber);
            return true;
        }

        public void GoToNextIn(BookmarkActionTarget target) =>
            GoToNextIn(filesSelectors[target]);

        private void GoToNextIn(Func<List<string>> getEligibleDocumentPaths)
        { 
            var targetDocuments = getEligibleDocumentPaths();

            var currentDocIndex = -1;
            if (currentTextDocumentPath != null &&
                (currentDocIndex = targetDocuments.IndexOf(currentTextDocumentPath)) != -1)
            {
                // Try to navigate within the same document; if success, we're done

                var success = GoToNextInDocument(currentTextDocumentPath, cycle: targetDocuments.Count < 2);
                if (success)
                    return;
            }

            if (!targetDocuments.Any())
            {
                Helpers.LogError("I have been asked to navigate to the previous bookmark in another file, but the list of target files I got is empty. Try closing and reopening the affected file(s).");
                return;
            }
            if (targetDocuments.Contains(null))
            {
                Helpers.LogError("I have been asked to navigate to the next bookmark in another file, but the list of target files contains \"null\".");
                return;
            }

            int nextDocIndex;
            if (currentDocIndex == targetDocuments.Count - 1)
                nextDocIndex = 0;
            else if (currentDocIndex == -1)
                nextDocIndex = targetDocuments.Count - 1;
            else
                nextDocIndex = currentDocIndex + 1;

            var nextDocPath = targetDocuments[nextDocIndex];

            if (!activeViewsByFilename.ContainsKey(nextDocPath) || //Doc is still in the "pending bookmarks" list...
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

            var nextDocView = activeViewsByFilename[nextDocPath];
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
        private bool GoToNextInDocument(string fileName, bool cycle)
        {
            var view = activeViewsByFilename[fileName];
            var buffer = view.TextBuffer;
            var bookmarks = bookmarksByView[view];

            if (bookmarks.Count == 0)
                return true;

            var currentLineNumber = GetCurrentLineNumber(view);
            var lineNumbers = bookmarks.Select(b => b.GetRow(buffer)).OrderBy(x => x).ToList();
            var nextLine = lineNumbers.FirstOrDefault(l => l > currentLineNumber);

            if (nextLine == 0 && !cycle)
                return false;

            var targetLineNumber =
                nextLine == 0
                    ? lineNumbers.First()
                    : nextLine;

            MoveToLineNumber(view, targetLineNumber);
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
