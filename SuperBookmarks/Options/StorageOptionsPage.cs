using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Konamiman.SuperBookmarks
{
    [Guid("04C2573A-1FFF-4210-AB38-8772EEDF603D")]
    public class StorageOptionsPage : OptionsPageBase
    {
        StorageOptionsControl control;

        public StorageOptionsPage()
        {
            control = new StorageOptionsControl();
            control.Options = this;

            control.OpenSuoFolderRequested += OnOpenSuoFolderRequested;
            control.IncludeFileInCurrentGitignoreRequested += OnIncludeFileInCurrentGitignoreRequested;
        }

        public bool SaveBookmarksToOwnFile { get; set; }
        
        public bool AutoIncludeInGitignore { get; set; }

        private void OnIncludeFileInCurrentGitignoreRequested(object sender, EventArgs eventArgs)
            => Helpers.SafeInvoke(_OnIncludeFileInCurrentGitignoreRequested);
        
        private void _OnIncludeFileInCurrentGitignoreRequested()
        {
            var gitignorePath = Path.Combine(
                Helpers.GetGitRepositoryRoot(SuperBookmarksPackage.Instance.CurrentSolutionPath),
                ".gitignore");

            if (!File.Exists(gitignorePath))
            {
                if(!Helpers.ShowYesNoQuestionMessage("The repository for the current solution has no .gitignore file, should I create one?"))
                    return;
            }

            if(Helpers.AddFileToGitignore(gitignorePath, SuperBookmarksPackage.Instance.DataFilePath, createGitignoreFile: true))
                Helpers.ShowInfoMessage("Ok, I have added .SuperBookmarks.dat file to .gitignore for the current solution.");
            else
                Helpers.ShowWarningMessage(".gitignore for the current solution already contains an entry for .SuperBookmarks.dat, so I did nothing.");
        }
        
        private void OnOpenSuoFolderRequested(object sender, EventArgs eventArgs)
        {
            Helpers.SafeInvoke(() =>
                Process.Start(Path.GetDirectoryName(SuperBookmarksPackage.Instance.CurrentSolutionSuoPath)));
        }


        protected override void OnActivate(CancelEventArgs e)
        {
            Helpers.SafeInvoke(control.Initialize);
            base.OnActivate(e);
        }

        protected override IWin32Window Window => control;

        public override void ResetSettings()
        {
            SaveBookmarksToOwnFile = false;
            AutoIncludeInGitignore = false;
        }

        public override void LoadSettingsFromStorage()
        {
            SaveBookmarksToOwnFile = LoadBooleanProperty("SaveBookmarksToOwnFile", false);
            AutoIncludeInGitignore = LoadBooleanProperty("AutoIncludeInGitignore", false);
        }

        public override void SaveSettingsToStorage()
        {
            SaveProperty("SaveBookmarksToOwnFile", SaveBookmarksToOwnFile);
            SaveProperty("AutoIncludeInGitignore", AutoIncludeInGitignore);
        }
    }
}
