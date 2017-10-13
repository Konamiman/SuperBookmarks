using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Editor;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        public PersistableBookmarksInfo GetPersistableInfo()
        {
            string RelativePath(string fullPath) =>
                fullPath.Substring(SolutionPath.Length);

            var info = new PersistableBookmarksInfo();

            //We shouldn't have any duplicate bookmark, but let's play safe
            var usedLineNumbers = new List<int>();

            foreach (var fileName in viewsByFilename.Keys)
            {
                var view = viewsByFilename[fileName];
                if (bookmarksByView[view].Count == 0) continue;

                var persistableBookmarks = new List<PersistableBookmarksInfo.Bookmark>();
                usedLineNumbers.Clear();
                foreach (var bookmark in bookmarksByView[view])
                {
                    var lineNumber = bookmark.GetRow(Helpers.GetRootTextBuffer(view.TextBuffer));
                    if (usedLineNumbers.Contains(lineNumber))
                        continue;

                    persistableBookmarks.Add(new PersistableBookmarksInfo.Bookmark
                    {
                        LineNumber = lineNumber
                    });
                    usedLineNumbers.Add(lineNumber);
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
