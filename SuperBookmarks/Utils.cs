using Microsoft.VisualStudio.Text.Editor;
using System.Linq;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        private int GetCurrentLineNumber(ITextView view)
           => view.Selection.ActivePoint.Position.GetContainingLine().LineNumber + 1;

        public bool HasBookmarks(string path)
        {
            return (viewsByFilename.ContainsKey(path) && bookmarksByView[viewsByFilename[path]].Any())
                   || bookmarksPendingCreation.ContainsKey(path);
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
