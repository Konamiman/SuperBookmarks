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

            return viewsByFilename.Keys.SingleOrDefault(EqualsIgnoreCase) ??
                bookmarksPendingCreation.Keys.SingleOrDefault(EqualsIgnoreCase);
        }

        public bool HasBookmarks(string path)
        {
            path = GetProperlyCasedRegisteredFilename(path);
            if (path == null) return false;

            return bookmarksPendingCreation.ContainsKey(path) ||
                    bookmarksByView[viewsByFilename[path]].Any();
        }

        public int GetBookmarksCount(BookmarkActionTarget target)
        {
            var count = 0;
            foreach (var path in filesSelectors[target]())
            {
                if (viewsByFilename.ContainsKey(path))
                    count += bookmarksByView[viewsByFilename[path]].Count;
                else if (bookmarksPendingCreation.ContainsKey(path))
                    count += bookmarksPendingCreation[path].Length;
            }

            return count;
        }
    }
}
