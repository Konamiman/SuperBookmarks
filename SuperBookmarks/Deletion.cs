namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        public void ClearAllBookmarks()
        {
            foreach (var fileName in activeViewsByFilename.Keys)
            {
                DeleteAllBookmarksInFile(fileName);
            }
            bookmarksByView.Clear();
            activeViewsByFilename.Clear();
            bookmarksPendingCreation.Clear();
        }

        private void DeleteAllBookmarksInFile(string path)
        {
            if (activeViewsByFilename.ContainsKey(path))
            {
                var view = activeViewsByFilename[path];
                var bookmarks = bookmarksByView[view];
                foreach (var bookmark in bookmarks.ToArray())
                    UnregisterAndDeleteBookmark(bookmarks, bookmark, view.TextBuffer);
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
