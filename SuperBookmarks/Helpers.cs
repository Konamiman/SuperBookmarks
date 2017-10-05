using System;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Konamiman.SuperBookmarks
{
    static class Helpers
    {
        private static IServiceProvider serviceProvider = null;

        private static IServiceProvider ServiceProvider =>
            serviceProvider ?? (serviceProvider = SuperBookmarksPackage.Instance);

        //https://msdn.microsoft.com/en-us/library/dd884850.aspx (AddAdornmentHandler)
        public static ITextView GetTextViewForActiveDocument()
        {
            IVsTextManager txtMgr = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
            IVsTextView vTextView = null;
            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);
            IVsUserData userData = vTextView as IVsUserData;
            if (userData == null)
            {
                throw new InvalidOperationException("No text view is currently open");
            }

            Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out var holder);
            IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;
            return viewHost.TextView;
        }

        //https://social.msdn.microsoft.com/Forums/en-US/0f6ef03a-df6b-4670-856e-f4a539fbfbe1/how-get-document-name-of-an-iwpftextview?forum=vseditor
        public static ITextDocument GetTextDocumentFor(ITextBuffer TextBuffer)
        {
            ITextDocument textDoc;
            var rc = TextBuffer.Properties.TryGetProperty<ITextDocument>(
              typeof(ITextDocument), out textDoc);
            if (rc == true)
                return textDoc;
            else
                return null;
        }

        public static TrackingTagSpan<BookmarkTag> CreateTagSpan(ITextBuffer buffer, int lineNumber)
        {
            var snapshot = buffer.CurrentSnapshot;

            //This can happen if the file is edited outside Visual Studio
            //while the solution is closed
            if (lineNumber > snapshot.LineCount)
                return null;

            var line = snapshot.GetLineFromLineNumber(lineNumber - 1);
            var span = snapshot.CreateTrackingSpan(new SnapshotSpan(line.Start, 0), SpanTrackingMode.EdgeExclusive);
            var tagger = GetTaggerFor(buffer);
            var trackingSpan = tagger.CreateTagSpan(span, new BookmarkTag());
            return trackingSpan;
        }
        
        public static SimpleTagger<BookmarkTag> GetTaggerFor(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty("tagger", () => new SimpleTagger<BookmarkTag>(buffer));
        }
    }
}
