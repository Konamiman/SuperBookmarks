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
