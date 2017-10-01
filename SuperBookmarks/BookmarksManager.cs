using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Konamiman.SuperBookmarks
{
    class BookmarksManager
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

        private readonly IServiceProvider serviceProvider;

        public BookmarksManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private readonly Dictionary<string, ITextView> viewsByFilename =
            new Dictionary<string, ITextView>();

        private readonly Dictionary<ITextView, List<Bookmark>> bookmarksByView =
            new Dictionary<ITextView, List<Bookmark>>();

        public void RegisterTextView(string fileName, ITextView newView)
        {
            if (viewsByFilename.ContainsKey(fileName))
            {
                //Recreate the existing bookmarks in the new view
                var oldView = viewsByFilename[fileName];
                var bookmarks = bookmarksByView[oldView];
                var currentBuffer = newView.TextBuffer;
                foreach (var bookmark in bookmarks)
                {
                    var lineNumber = bookmark.GetRow(oldView.TextBuffer);
                    var trackingSpan = Helpers.CreateTagSpan(currentBuffer, lineNumber);
                    bookmark.UpdateSpan(trackingSpan);
                }
                viewsByFilename[fileName] = newView;
                bookmarksByView.Remove(oldView);
                bookmarksByView[newView] = bookmarks;
            }
            else
            {
                viewsByFilename[fileName] = newView;
                bookmarksByView[newView] = new List<Bookmark>();
            }   
        }
        
        public void SetOrRemoveBookmarkInCurrentDocument()
        {
            var currentView = Helpers.GetTextViewForActiveDocument();
            var currentBuffer = currentView.TextBuffer;
            var lineNumber = currentView.Selection.ActivePoint.Position.GetContainingLine().LineNumber + 1;

            var existingBookmark = bookmarksByView[currentView].SingleOrDefault(b => b.GetRow(currentBuffer) == lineNumber);
            if (existingBookmark == null)
            {
                //Create new bookmark
                var trackingSpan = Helpers.CreateTagSpan(currentBuffer, lineNumber);
                bookmarksByView[currentView].Add(new Bookmark(trackingSpan));
            }
            else
            {
                //Remove existing bookmark
                var tagger = currentBuffer.Properties.GetOrCreateSingletonProperty(() => new SimpleTagger<BookmarkTag>(currentBuffer));
                tagger.RemoveTagSpan(existingBookmark.TrackingSpan); // s => s.Span == existingBookmark.TrackingSpan.Span);
                bookmarksByView[currentView].Remove(existingBookmark);
            }
        }
    }
}
