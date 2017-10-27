using Newtonsoft.Json;
using System.IO;
using System.Linq;

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

        public int TotalFilesCount =>
            BookmarksContexts
            .SelectMany(c => c.FilesWithBookmarks)
            .Count();
    }
}
