using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Konamiman.SuperBookmarks
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    class WpfTextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import]
        public ITextDocumentFactoryService documentService = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            var buffer = Helpers.GetRootTextBuffer(textView.TextBuffer);
            if (!documentService.TryGetTextDocument(buffer, out ITextDocument textDocument))
                return;

            var fileName = textDocument.FilePath;
            if(fileName != null)
                SuperBookmarksPackage.Instance.BookmarksManager.RegisterTextView(fileName, textView);
        }
    }
}
