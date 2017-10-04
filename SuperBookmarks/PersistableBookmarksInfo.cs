using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Konamiman.SuperBookmarks
{
    class PersistableBookmarksInfo
    {
        public Dictionary<string, Bookmark[]> BookmarksByFilename { get; }
            = new Dictionary<string, Bookmark[]>();

        public class Bookmark
        {
            public int LineNumber { get; set; }
        }
        
        public void SerializeTo(Stream stream)
        {
            var sb = new StringBuilder();
            foreach (var filename in BookmarksByFilename.Keys)
            {
                sb.Append(filename);
                sb.Append("?");
                sb.AppendLine(string.Join(",", BookmarksByFilename[filename].Select(b => b.LineNumber).ToArray()));
            }

            var serialized = sb.ToString();
            var writer = new StreamWriter(stream);
            writer.Write(serialized);
            writer.Dispose();
        }

        public static PersistableBookmarksInfo DeserializeFrom(Stream stream)
        {
            var reader = new StreamReader(stream);
            var serialized = reader.ReadToEnd();

            var lines = serialized.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var info = new PersistableBookmarksInfo();
            foreach (var line in lines)
            {
                var parts = line.Split('?');
                var bookmarks =
                    parts[1].Split(',').Select(l => new PersistableBookmarksInfo.Bookmark {LineNumber = int.Parse(l)});
                info.BookmarksByFilename[parts[0]] = bookmarks.ToArray();
            }

            reader.Dispose();
            return info;
        }
    }
}
