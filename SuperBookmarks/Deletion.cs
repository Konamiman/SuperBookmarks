using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        public void ClearAllBookmarks()
        {
            foreach (var fileName in viewsByFilename.Keys)
            {
                DeleteAllBookmarksInFile(fileName);
            }
            bookmarksByView.Clear();
            viewsByFilename.Clear();
            bookmarksPendingCreation.Clear();
        }

        private void DeleteAllBookmarksInFile(string path)
        {
            if (viewsByFilename.ContainsKey(path))
            {
                var view = viewsByFilename[path];
                foreach (var bookmark in bookmarksByView[view])
                {
                    var tagger = Helpers.GetTaggerFor(Helpers.GetRootTextBuffer(view.TextBuffer));
                    tagger.RemoveTagSpan(bookmark.TrackingSpan);
                }
                bookmarksByView[view].Clear();
                SolutionExplorerFilter.OnFileLostItsLastBookmark(path);
            }
            else if (bookmarksPendingCreation.ContainsKey(path))
            {
                bookmarksPendingCreation.Remove(path);
                SolutionExplorerFilter.OnFileLostItsLastBookmark(path);
            }
        }

        public void DeleteAllBookmarksIn(BookmarkActionTarget target)
        {
            var targetFiles = filesSelectors[target]();
            foreach (var file in targetFiles)
                DeleteAllBookmarksInFile(file);
        }
    }
}
