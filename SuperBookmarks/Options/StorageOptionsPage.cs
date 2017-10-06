using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace Konamiman.SuperBookmarks
{
    [Guid("04C2573A-1FFF-4210-AB38-8772EEDF603D")]
    public class StorageOptionsPage : DialogPage
    {
        StorageOptionsControl control;

        private bool optionsChanged = false;

        public StorageOptionsPage()
        {
            control = new StorageOptionsControl();
            control.Options = this;

            control.OpenSuoFolderRequested += OnOpenSuoFolderRequested;
            control.IncludeFileInCurrentGitignoreRequested += OnIncludeFileInCurrentGitignoreRequested;

            control.SaveOptionChanged += OnSaveOptionChanged;
        }

        private bool originalSaveBookmarksToOwnFile;
        public bool SaveBookmarksToOwnFile
        {
            get { return control.SaveInOwnFile; }
            set
            {
                control.SaveInOwnFile = value;
                control.SaveInSuoFile = !value;
                originalSaveBookmarksToOwnFile = value;
                OnSaveOptionChanged(control, EventArgs.Empty);
            }
        }

        private bool originalAutoIncludeInGitignore;
        public bool AutoIncludeInGitignore
        {
            get { return control.AutoIncludeInGitignore; }
            set
            {
                control.AutoIncludeInGitignore = value;
                originalAutoIncludeInGitignore = value;
            }
        }

        private void OnSaveOptionChanged(object sender, EventArgs eventArgs)
        {
            control.ChangeAutoGitignoreWriteEnabled(control.SaveInOwnFile);
            if (control.SaveInSuoFile)
                control.AutoIncludeInGitignore = false;

            optionsChanged = true;
        }

        private void OnIncludeFileInCurrentGitignoreRequested(object sender, EventArgs eventArgs)
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
            Process.Start(Path.GetDirectoryName(SuperBookmarksPackage.Instance.CurrentSolutionSuoPath));
        }

        public event EventHandler OptionsChanged;

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (optionsChanged)
                OptionsChanged?.Invoke(this, EventArgs.Empty);

            SetAsUnchanged();
            base.OnApply(e);
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            control.Initialize();
            SetAsUnchanged();

            var solutionIsOpen = SuperBookmarksPackage.Instance.SolutionIsCurrentlyOpen;
            var solutionIsInGitRepo = solutionIsOpen && SuperBookmarksPackage.Instance.CurrentSolutionIsInGitRepo;
            control.ChangeOpenSuoFolderEnabled(solutionIsOpen);
            control.ChangeIncludeInGitignoreNowEnabled(solutionIsInGitRepo);

            if(!solutionIsOpen)
                control.SetInfoMessage("No solution is open currently");
            else if(!solutionIsInGitRepo)
                control.SetInfoMessage("Current solution is not in a Git repository");
            else
                control.SetInfoMessage(null);

            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveBookmarksToOwnFile = originalSaveBookmarksToOwnFile;
            AutoIncludeInGitignore = originalAutoIncludeInGitignore;

            SetAsUnchanged();
            base.OnClosed(e);
        }

        private void SetAsUnchanged()
        {
            optionsChanged = false;
        }

        protected override IWin32Window Window => control;
    }
}
