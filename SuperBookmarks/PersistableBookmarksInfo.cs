using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Konamiman.SuperBookmarks
{
    class SerializableBookmarksInfo
    {
        public class FileWithBookmarks
        {
            public string FileName { get; set; }
            public int[] LinesWithBookmarks { get; set; }
        }

        public class BookmarksContext
        {
            public string Name { get; set; }
            public FileWithBookmarks[] FilesWithBookmarks { get; set; }
        }

        public BookmarksContext[] BookmarksContexts { get; set; }

        public void SerializeTo(Stream stream, bool prettyPrint)
        {
            var serializer = new JsonSerializer
            {
                Formatting = prettyPrint ? Formatting.Indented : Formatting.None
            };
            using (var writer = new StreamWriter(stream))
                serializer.Serialize(writer, this);
        }

        public static SerializableBookmarksInfo DeserializeFrom(Stream stream)
        {
            var deserializer = new JsonSerializer();
            using(var streamReader = new StreamReader(stream))
            using(var jsonReader = new JsonTextReader(streamReader))
                return deserializer.Deserialize<SerializableBookmarksInfo>(jsonReader);
        }

        public int TotalBookmarksCount =>
            BookmarksContexts
            .SelectMany(c => c.FilesWithBookmarks)
            .SelectMany(f => f.LinesWithBookmarks)
            .Count();
    }

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
