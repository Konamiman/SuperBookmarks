using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Konamiman.SuperBookmarks
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(BookmarkTag))]
    class BookmarkTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
            {
                Helpers.LogError("BookmarkTaggerProvider.CreateTagger invoked for null buffer");
                return null;
            }

            return (ITagger<T>)Helpers.SafeInvoke(() => Helpers.GetTaggerFor(buffer));
        }
    }
}
