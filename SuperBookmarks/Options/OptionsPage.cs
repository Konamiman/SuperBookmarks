using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        OptionsControl page;

        private bool deletingALineDeletesTheBookmark = true;
        private bool optionsChanged = false;
        private bool deletingALineDeletesTheBookmarkChanged = false;

        public OptionsPage()
        {
            page = new OptionsControl();
            page.Options = this;
        }

        public bool DeletingALineDeletesTheBookmark
        {
            get
            {
                return deletingALineDeletesTheBookmark;
            }
            set
            {
                deletingALineDeletesTheBookmark = value;
                deletingALineDeletesTheBookmarkChanged = true;
                optionsChanged = true;
            }
        }

        public event EventHandler DeletingALineDeletesTheBookmarkChanged;
        public event EventHandler OptionsChanged;

        protected override void OnApply(PageApplyEventArgs e)
        {
            if(deletingALineDeletesTheBookmarkChanged)
                DeletingALineDeletesTheBookmarkChanged?.Invoke(this, EventArgs.Empty);

            if(optionsChanged)
                OptionsChanged?.Invoke(this, EventArgs.Empty);

            SetAsUnchanged();
            base.OnApply(e);
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            page.Initialize();
            SetAsUnchanged();
            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            SetAsUnchanged();
            base.OnClosed(e);
        }

        private void SetAsUnchanged()
        {
            optionsChanged = false;
            deletingALineDeletesTheBookmarkChanged = false;
        }

        protected override IWin32Window Window => page;
    }
}
