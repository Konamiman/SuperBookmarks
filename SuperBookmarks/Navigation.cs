using Microsoft.VisualStudio.Text.Editor;
using System.Linq;

namespace Konamiman.SuperBookmarks
{
    partial class BookmarksManager
    {
        public void GoToPrevInCurrentDocument()
        {
            var docData = GetViewAndBookmarksForCurrentDocument();
            if (docData == null)
                return;

            if (docData.Bookmarks.Count == 0)
                return;

            var lineNumbers = docData.Bookmarks.Select(b => b.GetRow(docData.TextBuffer)).OrderByDescending(x => x).ToList();
            var prevLine = lineNumbers.FirstOrDefault(l => l < docData.CurrentLineNumber);
            var targetLineNumber =
                prevLine == 0
                    ? lineNumbers.First()
                    : prevLine;

            MoveToLineNumber(docData.TextView, targetLineNumber);
        }

        public void GoToNextInCurrentDocument()
        {
            var docData = GetViewAndBookmarksForCurrentDocument();
            if (docData == null)
                return;

            if (docData.Bookmarks.Count == 0)
                return;

            var lineNumbers = docData.Bookmarks.Select(b => b.GetRow(docData.TextBuffer)).OrderBy(x => x).ToList();
            var nextLine = lineNumbers.FirstOrDefault(l => l > docData.CurrentLineNumber);
            var targetLineNumber =
                nextLine == 0
                    ? lineNumbers.First()
                    : nextLine;

            MoveToLineNumber(docData.TextView, targetLineNumber);
        }

        private void MoveToLineNumber(ITextView textView, int lineNumber)
        {
            var point = textView.TextSnapshot.GetLineFromLineNumber(lineNumber - 1).Start;
            textView.Caret.MoveTo(point);
            if (textView is IWpfTextView)
            {
                var height = ((IWpfTextView)textView).VisualElement.ActualHeight;
                textView.DisplayTextLineContainingBufferPosition(point, height / 2, ViewRelativePosition.Top);
            }
            else
            {
                textView.Caret.EnsureVisible();
            }
        }
    }

}
