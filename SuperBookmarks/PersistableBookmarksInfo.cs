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
            public int[] Lines { get; set; }
        }

        public FileWithBookmarks[] FilesWithBookmarks { get; set; }

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

        [JsonIgnore]
        public int TotalBookmarksCount =>
            FilesWithBookmarks
            .SelectMany(f => f.Lines)
            .Count();

        [JsonIgnore]
        public int TotalFilesCount =>
            FilesWithBookmarks.Count();
    }
}
