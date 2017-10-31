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

        private string CurrentTextDocumentPath
        {
            get
            {
                return currentTextDocumentPath;
            }
            set
            {
                currentTextDocumentPath = value;
                currentTextDocumentPathCollection[0] = value;
            }
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

            if (viewsByFilename.ContainsKey(path))
                UnregisterTextView(path);

            CurrentTextDocumentPath = null;
        }

        public void OnSolutionClosed()
        {
            openDocumentPaths.Clear();
            currentDocumentFolder = null;
            CurrentTextDocumentPath = null;
            currentProjectFolder = null;
        }

        public void OnCurrentDocumentChanged(string path, string projectFolder)
        {
            currentDocumentFolder = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
            currentProjectFolder = projectFolder + Path.DirectorySeparatorChar;

            if (openDocumentPaths.Contains(path))
                CurrentTextDocumentPath = path;
            else
                CurrentTextDocumentPath = null;
        }

        public void OnFileDeleted(string filePath)
        {
            if(bookmarksPendingCreation.ContainsKey(filePath))
                bookmarksPendingCreation.Remove(filePath);

            if (viewsByFilename.ContainsKey(filePath))
                UnregisterTextView(filePath);

            if(openDocumentPaths.Contains(filePath))
                openDocumentPaths.Remove(filePath);

            if(filePath == CurrentTextDocumentPath)
                CurrentTextDocumentPath = null;
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

            if(openDocumentPaths.Contains(oldPath))
            {
                openDocumentPaths.Remove(oldPath);
                openDocumentPaths.Add(newPath);
            }

            if (oldPath == CurrentTextDocumentPath)
            {
                CurrentTextDocumentPath = newPath;
            }
        }
    }
}
