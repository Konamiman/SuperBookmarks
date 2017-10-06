using System.Windows.Forms;
using System;

namespace Konamiman.SuperBookmarks
{
    public partial class StorageOptionsControl : UserControl
    {
        private bool programmaticChangeInProgress = false;

        public StorageOptionsControl()
        {
            InitializeComponent();
        }

        internal StorageOptionsPage Options { get; set; }

        public void Initialize()
        {
            
        }

        public void ChangeAutoGitignoreWriteEnabled(bool enabled)
        {
            chkAutoIncludeInGitignore.Enabled = enabled;
            lblIfGitignoreExists.Enabled = enabled;
        }

        public void ChangeIncludeInGitignoreNowEnabled(bool enabled)
        {
            btnIncludeInGitignoreNow.Enabled = enabled;
        }

        public void ChangeOpenSuoFolderEnabled(bool enabled)
        {
            btnOpenSuoFolder.Enabled = enabled;
        }

        public bool SaveInSuoFile
        {
            get { return rbInSuo.Checked; }
            set
            {
                programmaticChangeInProgress = true;
                rbInSuo.Checked = value;
                programmaticChangeInProgress = false;
            }
        }

        public bool SaveInOwnFile
        {
            get { return rbInOwnFile.Checked; }
            set
            {
                programmaticChangeInProgress = true;
                rbInOwnFile.Checked = value;
                programmaticChangeInProgress = false;
            }
        }

        public bool AutoIncludeInGitignore
        {
            get { return chkAutoIncludeInGitignore.Checked; }
            set
            {
                programmaticChangeInProgress = true;
                chkAutoIncludeInGitignore.Checked = value;
                programmaticChangeInProgress = false;
            }
        }

        public void SetInfoMessage(string message)
        {
            if (message == null)
            {
                pnlInfoMessage.Visible = false;
            }
            else
            {
                lblInfoMessage.Text = message;
                pnlInfoMessage.Visible = true;
            }
        }

        public event EventHandler OpenSuoFolderRequested;
        public event EventHandler IncludeFileInCurrentGitignoreRequested;
        public event EventHandler SaveOptionChanged;

        private void btnOpenSuoFolder_Click(object sender, System.EventArgs e)
        {
            if(!programmaticChangeInProgress)
                OpenSuoFolderRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnIncludeInGitignoreNow_Click(object sender, EventArgs e)
        {
            if (!programmaticChangeInProgress)
                IncludeFileInCurrentGitignoreRequested?.Invoke(this, EventArgs.Empty);
        }

        private void rbInSuo_CheckedChanged(object sender, EventArgs e)
        {
            if (!programmaticChangeInProgress)
                SaveOptionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
