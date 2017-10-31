using System.ComponentModel;
using System.Windows.Forms;

namespace Konamiman.SuperBookmarks.Options
{
    public class DebugPage : OptionsPageBase
    {
        DebugOptionsControl control;

        public DebugPage()
        {
            control = new DebugOptionsControl();
            control.Options = this;
        }

        public bool ShowErrorsInMessageBox { get; set; }

        public bool ShowErrorsInOutputWindow { get; set; }

        public bool WriteErrorsToActivityLog { get; set; }

        protected override IWin32Window Window => control;

        protected override void OnActivate(CancelEventArgs e)
        {
            control.Initialize();
            base.OnActivate(e);
        }

        public override void ResetSettings()
        {
            ShowErrorsInMessageBox = false;
            ShowErrorsInOutputWindow = false;
            WriteErrorsToActivityLog = true;
        }

        public override void SaveSettingsToStorage()
        {
            SaveProperty("ShowErrorsInMessageBox", ShowErrorsInMessageBox);
            SaveProperty("ShowErrorsInOutputWindow", ShowErrorsInOutputWindow);
            SaveProperty("WriteErrorsToActivityLog", WriteErrorsToActivityLog);
        }

        public override void LoadSettingsFromStorage()
        {
            ShowErrorsInMessageBox = LoadBooleanProperty("ShowErrorsInMessageBox", false);
            ShowErrorsInOutputWindow = LoadBooleanProperty("ShowErrorsInOutputWindow", false);
            WriteErrorsToActivityLog = LoadBooleanProperty("WriteErrorsToActivityLog", true);
        }
    }
}
