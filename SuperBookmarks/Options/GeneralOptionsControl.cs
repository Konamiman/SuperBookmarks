using System;
using System.Drawing;
using System.Windows.Forms;

namespace Konamiman.SuperBookmarks
{
    public partial class GeneralOptionsControl : UserControl
    {
        public GeneralOptionsControl()
        {
            InitializeComponent();
        }

        internal GeneralOptionsPage Options { get; set; }

        public void Initialize()
        {
            colorDialog.Color = Options.GlyphColor;
            pnlChooseColor.BackColor = colorDialog.Color;

            if (Options.CustomColors != null)
                colorDialog.CustomColors = Options.CustomColors;

            chkDeletingLineDeletesBookmark.Checked = Options.DeletingALineDeletesTheBookmark;

            switch(Options.ShowMenuOption)
            {
                case ShowMenuOption.WithTitleSuperBookmarks:
                    rbMenuShowSuperBookmarks.Checked = true;
                    break;
                case ShowMenuOption.WithTitleBookmarks:
                    rbMenuShowBookmarks.Checked = true;
                    break;
                case ShowMenuOption.DontShow:
                    rbMenuDontShow.Checked = true;
                    break;
            }

            if (Options.MergeWhenImporting)
                rbImportMerges.Checked = true;
            else
                rbImportReplaces.Checked = true;

            chkNavInFolderIncludesSubfolders.Checked = Options.NavigateInFolderIncludesSubfolders;
            chkDelAllInFolderIncludesSubfolder.Checked = Options.DeleteAllInFolderIncludesSubfolders;

            chkDeletingLineDeletesBookmark.CheckedChanged += chkDeletingLineDeletesBookmark_CheckedChanged;
            chkNavInFolderIncludesSubfolders.CheckedChanged += ChkNavInFolderIncludesSubfoldersOnCheckedChanged;
            chkDelAllInFolderIncludesSubfolder.CheckedChanged += ChkDelAllInFolderIncludesSubfolderOnCheckedChanged;
            rbImportMerges.CheckedChanged += rbImportMerges_CheckedChanged;

            rbMenuShowSuperBookmarks.CheckedChanged += MenuShowSuperBookmarks_CheckedChanged;
            rbMenuShowBookmarks.CheckedChanged += MenuShowSuperBookmarks_CheckedChanged;
            rbMenuDontShow.CheckedChanged += MenuShowSuperBookmarks_CheckedChanged;
        }

        private void MenuShowSuperBookmarks_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked) return;
            if (rbMenuShowSuperBookmarks.Checked)
                Options.ShowMenuOption = ShowMenuOption.WithTitleSuperBookmarks;
            else if (rbMenuShowBookmarks.Checked)
                Options.ShowMenuOption = ShowMenuOption.WithTitleBookmarks;
            else if (rbMenuDontShow.Checked)
                Options.ShowMenuOption = ShowMenuOption.DontShow;
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

        private void rbImportMerges_CheckedChanged(object sender, EventArgs e)
        {
            Options.MergeWhenImporting = rbImportMerges.Checked;
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
