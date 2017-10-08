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
            pnlRequiresRestaring.Visible = false;

            chkDeletingLineDeletesBookmark.Checked = Options.DeletingALineDeletesTheBookmark;
            if (Options.ShowCommandsInTopLevelMenu)
                rbInTopLevel.Checked = true;
            else
                rbInEdit.Checked = true;

            chkDeletingLineDeletesBookmark.CheckedChanged += chkDeletingLineDeletesBookmark_CheckedChanged;
            rbInTopLevel.CheckedChanged += rbInTopLevelMenu_CheckedChanged;
        }

        private void chkDeletingLineDeletesBookmark_CheckedChanged(object sender, EventArgs e)
        {
            Options.DeletingALineDeletesTheBookmark = chkDeletingLineDeletesBookmark.Checked;
        }

        private void rbInTopLevelMenu_CheckedChanged(object sender, EventArgs e)
        {
            Options.ShowCommandsInTopLevelMenu = rbInTopLevel.Checked;
            pnlRequiresRestaring.Visible = true;
        }
    }
}
