using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Linq;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        private int GetCurrentLineNumber(ITextView view)
           => view.Selection.ActivePoint.Position.GetContainingLine().LineNumber + 1;

        private string GetProperlyCasedRegisteredFilename(string fileNameWithMismatchingCase)
        {
            bool EqualsIgnoreCase(string value1) =>
                value1.Equals(fileNameWithMismatchingCase, StringComparison.OrdinalIgnoreCase);

            return activeViewsByFilename.Keys.SingleOrDefault(EqualsIgnoreCase) ??
                bookmarksPendingCreation.Keys.SingleOrDefault(EqualsIgnoreCase);
        }

        public bool HasBookmarks(string path)
        {
            path = GetProperlyCasedRegisteredFilename(path);
            if (path == null) return false;

            return bookmarksPendingCreation.ContainsKey(path) ||
                    bookmarksByView[activeViewsByFilename[path]].Any();
        }

        public int GetBookmarksCount(BookmarkActionTarget target)
        {
            var count = 0;
            foreach (var path in filesSelectors[target]())
            {
                if (activeViewsByFilename.ContainsKey(path))
                    count += bookmarksByView[activeViewsByFilename[path]].Count;
                else if (bookmarksPendingCreation.ContainsKey(path))
                    count += bookmarksPendingCreation[path].Length;
            }

            return count;
        }

        public int GetFilesWithBookmarksCount()
            => bookmarksPendingCreation.Count + 
               bookmarksByView.Values.Count(b => b.Any());

        public int GetFilesWithBookmarksCount(BookmarkActionTarget target = BookmarkActionTarget.Solution)
        {
            var targetFiles = filesSelectors[target]();

            var pendingCreationCount = bookmarksPendingCreation.Keys.Intersect(targetFiles).Count();
            var registeredCount = activeViewsByFilename.Keys.Intersect(targetFiles)
                .Where(f => bookmarksByView[activeViewsByFilename[f]].Any()).Count();

            return pendingCreationCount + registeredCount;
        }
    }
}
