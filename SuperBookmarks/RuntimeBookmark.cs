using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        class Bookmark
        {
            public TrackingTagSpan<BookmarkTag> TrackingSpan { get; private set; }

            public Bookmark(TrackingTagSpan<BookmarkTag> trackingSpan)
            {
                this.TrackingSpan = trackingSpan;
            }

            public void UpdateSpan(TrackingTagSpan<BookmarkTag> span)
            {
                this.TrackingSpan = span;
            }

            //This is set when the buffer is changing, needed to properly handling line deletes
            public int LineNumberBeforeChanging { get; set; }

            public int GetRow(ITextBuffer buffer)
            {
                return TrackingSpan.Span
                  .GetStartPoint(buffer.CurrentSnapshot)
                  .GetContainingLine()
                  .LineNumber + 1;
            }
        }
    }
}
