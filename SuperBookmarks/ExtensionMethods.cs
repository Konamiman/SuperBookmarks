using System.IO;

namespace Konamiman.SuperBookmarks
{
    public static class ExtensionMethods
    {
        public static string WithTrailingDirectorySeparator(this string value)
        {
            return string.IsNullOrEmpty(value) || value[value.Length - 1] == Path.DirectorySeparatorChar ?
                value :
                value + Path.DirectorySeparatorChar;
        }
    }
}
