using System.Collections.Generic;
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

        public void OnTextDocumentOpen(string path)
        {
            if (!openDocumentPaths.Contains(path))
                openDocumentPaths.Add(path);
        }

        public void OnTextDocumentClosed(string path)
        {
            if (openDocumentPaths.Contains(path))
                openDocumentPaths.Remove(path);

            if (viewsByFilename.ContainsKey(path))
                UnregisterTextView(path);

            currentTextDocumentPath = null;
            currentTextDocumentPathCollection[0] = null;
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
                currentTextDocumentPath = path;
            else
                currentTextDocumentPath = null;

            currentTextDocumentPathCollection[0] = currentTextDocumentPath;
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
    }
}
