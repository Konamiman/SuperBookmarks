using System;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace Konamiman.SuperBookmarks.Options
{
    public partial class DebugOptionsControl : UserControl
    {
        public DebugOptionsControl()
        {
            InitializeComponent();
        }
        
        internal DebugPage Options { get; set; }

        public void Initialize()
        {
            chkShowErrorsInMessageBox.Checked = Options.ShowErrorsInMessageBox;
            chkWriteErrorsToOutput.Checked = Options.ShowErrorsInOutputWindow;
            chkLogErrorsInActivityLog.Checked = Options.WriteErrorsToActivityLog;

            chkShowErrorsInMessageBox.CheckedChanged += ChkShowErrorsInMessageBox_CheckedChanged;
            chkWriteErrorsToOutput.CheckedChanged += ChkWriteErrorsToOutput_CheckedChanged;
            chkLogErrorsInActivityLog.CheckedChanged += ChkLogErrorsInActivityLog_CheckedChanged;
        }

        private void ChkLogErrorsInActivityLog_CheckedChanged(object sender, EventArgs e)
        {
            Options.WriteErrorsToActivityLog = chkLogErrorsInActivityLog.Checked;
        }

        private void ChkWriteErrorsToOutput_CheckedChanged(object sender, EventArgs e)
        {
            Options.ShowErrorsInOutputWindow = chkWriteErrorsToOutput.Checked;
        }

        private void ChkShowErrorsInMessageBox_CheckedChanged(object sender, EventArgs e)
        {
            Options.ShowErrorsInMessageBox = chkShowErrorsInMessageBox.Checked;
        }

        private bool ActivityLogExists()
        {
            if(Helpers.ActivityLogFilePath == null || !File.Exists(Helpers.ActivityLogFilePath))
            {
                Helpers.ShowInfoMessage("That's weird, but the Activity Log file doesn't exist.");
                return false;
            }

            return true;
        }

        private void btnOpenActivityLog_Click(object sender, EventArgs e)
        {
            if(ActivityLogExists())
                Process.Start(Helpers.ActivityLogFilePath);
        }

        private void btnCopyActivityLogPath_Click(object sender, EventArgs e)
        {
            if (ActivityLogExists())
                Clipboard.SetText(Helpers.ActivityLogFilePath);
        }

        private void btnThrowTestException_Click(object sender, EventArgs e)
        {
#if DEBUG
            try
            {
                throw new TestException();
            }
            catch(Exception ex)
            {
                Helpers.LogException(ex);
            }
#else
            throw new TestException();
#endif
        }

        private class TestException : Exception
        {
            public TestException() : base ($"{DateTime.Now:H:mm:ss} - Test exception thrown manually from the Options - Debug fialog.")
            {
            }
        }
    }
}
