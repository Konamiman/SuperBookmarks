using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        public SerializableBookmarksInfo GetSerializableInfo()
        {
            string ConvertPath(string fullPath) =>
                fullPath.StartsWith(SolutionPath) ?
                fullPath.Substring(SolutionPath.Length) :
                fullPath;

            var filesWithBookmarks = new List<SerializableBookmarksInfo.FileWithBookmarks>();
            var lineNumbers = new List<int>();

            //We shouldn't have any duplicate bookmark, but let's play safe
            var usedLineNumbers = new List<int>();

            foreach (var fileName in activeViewsByFilename.Keys)
            {
                var view = activeViewsByFilename[fileName];
                if (bookmarksByView[view].Count == 0) continue;

                lineNumbers.Clear();
                usedLineNumbers.Clear();
                foreach (var bookmark in bookmarksByView[view])
                {
                    var lineNumber = bookmark.GetRow(Helpers.GetRootTextBuffer(view.TextBuffer));
                    if (usedLineNumbers.Contains(lineNumber))
                        continue;

                    lineNumbers.Add(lineNumber);
                    usedLineNumbers.Add(lineNumber);
                }

                filesWithBookmarks.Add(new SerializableBookmarksInfo.FileWithBookmarks
                {
                    FileName = ConvertPath(fileName),
                    LinesWithBookmarks = lineNumbers.ToArray()
                });
            }

            foreach (var fileName in bookmarksPendingCreation.Keys)
            {
                filesWithBookmarks.Add(new SerializableBookmarksInfo.FileWithBookmarks
                {
                    FileName = ConvertPath(fileName),
                    LinesWithBookmarks = bookmarksPendingCreation[fileName].ToArray()
                });
            }

            var info = new SerializableBookmarksInfo();
            var context = new SerializableBookmarksInfo.BookmarksContext
            {
                Name = "(default)",
                FilesWithBookmarks = filesWithBookmarks.ToArray()
            };
            info.BookmarksContexts = new[] { context };

            return info;
        }

        public void RecreateBookmarksFromSerializedInfo(SerializableBookmarksInfo info)
        {
            foreach (var item in info.BookmarksContexts[0].FilesWithBookmarks)
            {
                var fileName = Path.Combine(SolutionPath, item.FileName); //Filename could be absolute, then this is a NOP, that's ok
                if (!File.Exists(fileName)) continue;

                var lineNumbers = item.LinesWithBookmarks;

                if (activeViewsByFilename.ContainsKey(fileName))
                {
                    var view = activeViewsByFilename[fileName];
                    var buffer = view.TextBuffer;
                    var bookmarks = bookmarksByView[view];
                    foreach (var lineNumber in lineNumbers)
                    {
                        var bookmarkExists = bookmarks.Any(b => b.GetRow(buffer) == lineNumber);
                        if (!bookmarkExists)
                            RegisterAndCreateBookmark(bookmarks, buffer, fileName, lineNumber);
                    }
                }
                else if(bookmarksPendingCreation.ContainsKey(fileName))
                {
                    bookmarksPendingCreation[fileName] = bookmarksPendingCreation[fileName].Union(lineNumbers).ToArray();
                }
                else
                {
                    bookmarksPendingCreation.Add(fileName, lineNumbers);
                    SolutionExplorerFilter.OnFileGotItsFirstBookmark(fileName);
                }
            }
        }
    }
}
