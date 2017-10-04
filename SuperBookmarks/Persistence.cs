using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        public PersistableBookmarksInfo GetPersistableInfo()
        {
            string RelativePath(string fullPath) =>
                fullPath.Substring(SolutionPath.Length);

            var info = new PersistableBookmarksInfo();

            foreach (var fileName in viewsByFilename.Keys)
            {
                var view = viewsByFilename[fileName];
                if (bookmarksByView[view].Count == 0) continue;

                var persistableBookmarks = new List<PersistableBookmarksInfo.Bookmark>();
                foreach (var bookmark in bookmarksByView[view])
                {
                    persistableBookmarks.Add(new PersistableBookmarksInfo.Bookmark
                    {
                        LineNumber = bookmark.GetRow(view.TextBuffer)
                    });
                }
                info.BookmarksByFilename[RelativePath(fileName)] = persistableBookmarks.ToArray();
            }

            foreach (var filename in bookmarksPendingCreation.Keys)
            {
                var persistableBookmarks =
                    bookmarksPendingCreation[filename]
                    .Select(l => new PersistableBookmarksInfo.Bookmark { LineNumber = l });
                info.BookmarksByFilename[RelativePath(filename)] = persistableBookmarks.ToArray();
            }

            return info;
        }

        public void RecreateBookmarksFromPersistableInfo(PersistableBookmarksInfo info)
        {
            ClearAllBookmarks();

            foreach (var fileName in info.BookmarksByFilename.Keys)
            {
                var lineNumbers = info.BookmarksByFilename[fileName].Select(b => b.LineNumber).ToArray();
                bookmarksPendingCreation[Path.Combine(SolutionPath, fileName)] = lineNumbers;
            }
        }
    }
}
