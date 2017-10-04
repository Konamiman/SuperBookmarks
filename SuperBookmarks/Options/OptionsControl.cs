using System;
using System.Windows.Forms;

namespace Konamiman.SuperBookmarks
{
    public partial class OptionsControl : UserControl
    {
        public OptionsControl()
        {
            InitializeComponent();
        }

        internal OptionsPage Options { get; set; }

        public void Initialize()
        {
            chkDeletingLineDeletesBookmark.Checked = Options.DeletingALineDeletesTheBookmark;
        }

        private void chkDeletingLineDeletesBookmark_CheckedChanged(object sender, EventArgs e)
        {
            Options.DeletingALineDeletesTheBookmark = chkDeletingLineDeletesBookmark.Checked;
        }
    }
}
