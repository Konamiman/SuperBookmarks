using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Konamiman.SuperBookmarks
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(BookmarkTag))]
    class BookmarkTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassifierAggregatorService AggregatorService;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            return (ITagger<T>)Helpers.GetTaggerFor(buffer);
        }
    }
}
