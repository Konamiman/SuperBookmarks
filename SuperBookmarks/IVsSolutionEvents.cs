using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public string DataFilePath =>
            Path.Combine(Path.GetDirectoryName(CurrentSolutionSuoPath), ".SuperBookmarks.dat");

        public int OnAfterCloseSolution(object pUnkReserved) =>
            Helpers.SafeInvoke(_OnAfterCloseSolution);

        private int _OnAfterCloseSolution()
        {
            SaveBookmarksToDatFile();
            this.BookmarksManager.ClearAllBookmarks();
            this.BookmarksManager.OnSolutionClosed();

            SolutionIsCurrentlyOpen = false;
            ActiveDocumentIsText = false;
            ThereAreOpenTextDocuments = false;
            ActiveDocumentIsInProject = false;

            TearDownRunningDocumentsInfo();
            Helpers.ClearProperlyCasedPathsCache();

            return VSConstants.S_OK;
        }

        public void SaveBookmarksToDatFile()
        {
            if (!StorageOptions.SaveBookmarksToOwnFile)
                return;

            var info = BookmarksManager.GetSerializableInfo();
            using (var stream = File.Create(DataFilePath))
                info.SerializeTo(stream, prettyPrint: false);

            Helpers.WriteToStatusBar($"Saved {Helpers.Quantifier(info.TotalBookmarksCount, "bookmark")} from {Helpers.Quantifier(info.TotalFilesCount, "file")} to .SuperBookmarks.dat file");
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) =>
            Helpers.SafeInvoke(_OnAfterOpenSolution);

        private int _OnAfterOpenSolution()
        {
            InitializeSolutionInfo();
            LoadBookmarksFromDatFile();
            AddDatFileToGitignore();

            return VSConstants.S_OK;
        }

        private void InitializeSolutionInfo()
        {
            solutionService.GetSolutionInfo(out string solutionPath, out string solutionFilePath, out string suoPath);
            this.BookmarksManager.SolutionPath = solutionPath;
            CurrentSolutionPath = solutionPath;
            CurrentSolutionSuoPath = suoPath;
            CurrentSolutionIsInGitRepo = Helpers.PathIsInGitRepository(solutionPath);
            SolutionIsCurrentlyOpen = true;
        }

        private void AddDatFileToGitignore()
        {
            if (StorageOptions.AutoIncludeInGitignore && Helpers.PathIsInGitRepository(CurrentSolutionPath))
            {
                var gitignorePath = Path.Combine(
                    Helpers.GetGitRepositoryRoot(CurrentSolutionPath),
                    ".gitignore");
                Helpers.AddFileToGitignore(gitignorePath, DataFilePath, createGitignoreFile: false);
            }
        }

        public void LoadBookmarksFromDatFile()
        {
            if (!StorageOptions.SaveBookmarksToOwnFile)
                return;

            if (!File.Exists(DataFilePath))
                return;

            SerializableBookmarksInfo info;
            try
            {
                using (var stream = File.OpenRead(DataFilePath))
                    info = SerializableBookmarksInfo.DeserializeFrom(stream);
            }
            catch
            {
                Helpers.ShowErrorMessage("Sorry, I couldn't parse the .SuperBookmarks.dat file, perhaps it is corrupted?", showHeader: false);
                return;
            }

            this.BookmarksManager.DeleteAllBookmarksIn(BookmarkActionTarget.Solution);

            try
            {
                this.BookmarksManager.RecreateBookmarksFromSerializedInfo(info);
            }
            catch
            {
                Helpers.ShowErrorMessage("Sorry, I couldn't get bookmarks data the .SuperBookmarks.dat file, perhaps it is corrupted?", showHeader: false);
                return;
            }
                        
            Helpers.WriteToStatusBar($"Restored {Helpers.Quantifier(info.TotalBookmarksCount, "bookmark")} for {Helpers.Quantifier(info.TotalFilesCount, "file")} from .SuperBookmarks.dat file");
        }
        
        #region Unused members

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
