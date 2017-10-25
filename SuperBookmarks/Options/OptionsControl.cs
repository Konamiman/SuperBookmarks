using System;
using System.Drawing;
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
            colorDialog.Color = Options.GlyphColor;
            pnlChooseColor.BackColor = colorDialog.Color;

            if (Options.CustomColors != null)
                colorDialog.CustomColors = Options.CustomColors;

            pnlRequiresRestaring.Visible = false;

            chkDeletingLineDeletesBookmark.Checked = Options.DeletingALineDeletesTheBookmark;
            if (Options.ShowCommandsInTopLevelMenu)
                rbInTopLevel.Checked = true;
            else
                rbInEdit.Checked = true;

            chkNavInFolderIncludesSubfolders.Checked = Options.NavigateInFolderIncludesSubfolders;
            chkDelAllInFolderIncludesSubfolder.Checked = Options.DeleteAllInFolderIncludesSubfolders;

            chkDeletingLineDeletesBookmark.CheckedChanged += chkDeletingLineDeletesBookmark_CheckedChanged;
            chkNavInFolderIncludesSubfolders.CheckedChanged += ChkNavInFolderIncludesSubfoldersOnCheckedChanged;
            chkDelAllInFolderIncludesSubfolder.CheckedChanged += ChkDelAllInFolderIncludesSubfolderOnCheckedChanged;
            rbInTopLevel.CheckedChanged += rbInTopLevelMenu_CheckedChanged;
        }

        private void chkDeletingLineDeletesBookmark_CheckedChanged(object sender, EventArgs e)
        {
            Options.DeletingALineDeletesTheBookmark = chkDeletingLineDeletesBookmark.Checked;
        }
        
        private void ChkNavInFolderIncludesSubfoldersOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            Options.NavigateInFolderIncludesSubfolders = chkNavInFolderIncludesSubfolders.Checked;
        }

        private void ChkDelAllInFolderIncludesSubfolderOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            Options.DeleteAllInFolderIncludesSubfolders = chkDelAllInFolderIncludesSubfolder.Checked;
        }

        private void rbInTopLevelMenu_CheckedChanged(object sender, EventArgs e)
        {
            Options.ShowCommandsInTopLevelMenu = rbInTopLevel.Checked;
            pnlRequiresRestaring.Visible = true;
        }

        private void pnlChooseColor_Click(object sender, EventArgs e)
        {
            OnColorDialogRequested();
        }

        private void lblGlyphColor_Click(object sender, EventArgs e)
        {
            OnColorDialogRequested();
        }

        private void OnColorDialogRequested()
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                SetGlypColor(colorDialog.Color);
                Options.CustomColors = colorDialog.CustomColors;
            }
        }

        private void btnResetColor_Click(object sender, EventArgs e)
        {
            SetGlypColor(BookmarkGlyphFactory.DefaultColor);
        }

        private void SetGlypColor(Color color)
        {
            pnlChooseColor.BackColor = color;
            Options.GlyphColor = color;
        }
    }
}
