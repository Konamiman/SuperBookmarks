using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Linq;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        private void TextBufferOnChanging(object sender, TextContentChangingEventArgs eventArgs)
        {
            var buffer = Helpers.GetRootTextBuffer(eventArgs.Before.TextBuffer);
            var view = buffer.Properties["view"] as ITextView;
            var bookmarks = bookmarksByView[view];
            foreach (var bookmark in bookmarks)
                bookmark.LineNumberBeforeChanging = bookmark.GetRow(buffer);
        }

        private void TextBufferChanged(object sender, TextContentChangedEventArgs eventArgs)
        {
            bool LineIsEntirelyContainedInChange(ITextSnapshotLine line, ITextChange change)
            {
                return (
                    line.Start.Position >= change.OldPosition &&
                    line.EndIncludingLineBreak.Position <= change.OldEnd) ||

                //This is needed for handling the deletion of blank lines

                (change.OldText == "\r\n" && change.NewText == "" &&
                    change.OldEnd == line.EndIncludingLineBreak.Position);
            }

            if (!eventArgs.Changes.IncludesLineChanges)
                return;

            var lineDeletionChanges = eventArgs.Changes.Where(ch => ch.LineCountDelta < 0).ToArray();
            if (lineDeletionChanges.Length == 0)
                return;

            var lines = eventArgs.Before.Lines.ToArray();
            var buffer = (ITextBuffer)sender;
            var view = buffer.Properties["view"] as ITextView;
            var bookmarks = bookmarksByView[view];

            if (!deletingALineDeletesTheBookmark)
            {
                // If "deleting a line deletes the bookmark" is off, we will end up
                // with duplicate bookmakrs when lines are deleted
                // (e.g. you have bookmarks in lines 3 and 4, you delete line 3,
                // now you have two bookmarks for line 3).
                var duplicateBookmarkGroup = bookmarks.GroupBy(b => b.GetRow(buffer)).Where(g => g.Count() > 1);
                foreach (var group in duplicateBookmarkGroup)
                {
                    var bookmarksInGroup = group.ToArray();
                    for (int i = 1; i < bookmarksInGroup.Length; i++)
                        bookmarks.Remove(bookmarksInGroup[i]);
                }
                return;
            }

            foreach (var change in lineDeletionChanges)
            {
                var deletedLines = lines.Where(l => LineIsEntirelyContainedInChange(l, change));
                var deletedLineNumbers = deletedLines.Select(l => l.LineNumber + 1).ToArray();

                foreach (var deletedLineNumber in deletedLineNumbers)
                {
                    var matchingBookmark = bookmarks.SingleOrDefault(b => b.LineNumberBeforeChanging == deletedLineNumber);
                    if (matchingBookmark == null)
                        continue;

                    UnregisterAndDeleteBookmark(bookmarks, matchingBookmark, buffer);
                }
            }
        }
    }
}
