using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks
{
    [Guid("BC1182FD-1E47-4616-BFC8-062683FB207A")]
    public class OptionsPage : DialogPage
    {
        private bool deletingALineDeletesTheBookmark = true;

        public bool DeletingALineDeletesTheBookmark
        {
            get
            {
                return deletingALineDeletesTheBookmark;
            }
            set
            {
                deletingALineDeletesTheBookmark = value;
                DeletingALineDeletesTheBookmarkChanged?.Invoke(this, EventArgs.Empty);
                OptionsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler DeletingALineDeletesTheBookmarkChanged;
        public event EventHandler OptionsChanged;

        protected override IWin32Window Window
        {
            get
            {
                OptionsControl page = new OptionsControl();
                page.Options = this;
                page.Initialize();
                return page;
            }
        }
    }
}
