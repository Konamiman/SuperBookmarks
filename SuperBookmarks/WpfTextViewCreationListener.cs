using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Konamiman.SuperBookmarks
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    class WpfTextViewCreationListener : IWpfTextViewCreationListener
    {
        public void TextViewCreated(IWpfTextView textView)
        {
            var fileName = Helpers.GetTextDocumentFor(textView.TextBuffer).FilePath;
            SuperBookmarksPackage.Instance.BookmarksManager.RegisterTextView(fileName, textView);
        }
    }
}
