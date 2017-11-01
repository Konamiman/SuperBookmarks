using System.Windows.Forms;
using System;

namespace Konamiman.SuperBookmarks
{
    public partial class StorageOptionsControl : UserControl
    {
        public StorageOptionsControl()
        {
            InitializeComponent();
        }

        internal StorageOptionsPage Options { get; set; }

        public void Initialize()
        {
            rbInOwnFile.Checked = Options.SaveBookmarksToOwnFile;
            rbInSuo.Checked = !Options.SaveBookmarksToOwnFile;
            chkAutoIncludeInGitignore.Checked = Options.AutoIncludeInGitignore;

            rbInOwnFile.CheckedChanged += RbInOwnFile_CheckedChanged;
            chkAutoIncludeInGitignore.CheckedChanged += ChkAutoIncludeInGitignore_CheckedChanged;

            var solutionIsOpen = SuperBookmarksPackage.Instance.SolutionIsCurrentlyOpen;
            var solutionIsInGitRepo = solutionIsOpen && SuperBookmarksPackage.Instance.CurrentSolutionIsInGitRepo;
            SetControlsState(solutionIsOpen, solutionIsInGitRepo);
        }
        
        private void SetControlsState(bool solutionIsOpen, bool solutionIsInGitRepo)
        {
            btnIncludeInGitignoreNow.Enabled = solutionIsInGitRepo;
            btnOpenSuoFolder.Enabled = solutionIsOpen;

            if (!solutionIsOpen)
                lblInfoMessage.Text = "No solution is open currently";
            else if (!solutionIsInGitRepo)
                lblInfoMessage.Text = "Current solution is not in a Git repository";
            else
                lblInfoMessage.Text = "";

            pnlInfoMessage.Visible = lblInfoMessage.Text != "";
        }

        private void RbInOwnFile_CheckedChanged(object sender, EventArgs e)
        {
            var saveToOwnFile = rbInOwnFile.Checked;

            Options.SaveBookmarksToOwnFile = saveToOwnFile;

            chkAutoIncludeInGitignore.Enabled = saveToOwnFile;
            lblIfGitignoreExists.Enabled = saveToOwnFile;

            if (!saveToOwnFile)
                chkAutoIncludeInGitignore.Checked = false;
        }
        
        private void ChkAutoIncludeInGitignore_CheckedChanged(object sender, EventArgs e)
        {
            Options.AutoIncludeInGitignore = chkAutoIncludeInGitignore.Checked;
        }

        public event EventHandler OpenSuoFolderRequested;
        public event EventHandler IncludeFileInCurrentGitignoreRequested;

        private void btnOpenSuoFolder_Click(object sender, System.EventArgs e)
        {
            OpenSuoFolderRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnIncludeInGitignoreNow_Click(object sender, EventArgs e)
        {
            IncludeFileInCurrentGitignoreRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
