using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public string DataFilePath =>
            Path.Combine(Path.GetDirectoryName(CurrentSolutionSuoPath), ".SuperBookmarks.dat");

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            if (StorageOptions.SaveBookmarksToOwnFile)
            {
                var info = this.BookmarksManager.GetPersistableInfo();
                using(var stream = File.Create(DataFilePath))
                    info.SerializeTo(stream);
            }

            this.BookmarksManager.ClearAllBookmarks();
            SolutionIsCurrentlyOpen = false;
            ActiveDocumentIsText = false;

            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            solutionService.GetSolutionInfo(out string solutionPath, out string solutionFilePath, out string suoPath);
            CurrentSolutionPath = solutionPath;
            CurrentSolutionSuoPath = suoPath;
            CurrentSolutionIsInGitRepo = Helpers.PathIsInGitRepository(solutionPath);
            SolutionIsCurrentlyOpen = true;

            if (StorageOptions.SaveBookmarksToOwnFile)
            {
                if (!File.Exists(DataFilePath))
                {
                    this.BookmarksManager.ClearAllBookmarks();
                    return VSConstants.S_OK;
                }

                using (var stream = File.OpenRead(DataFilePath))
                {
                    var info = PersistableBookmarksInfo.DeserializeFrom(stream);
                    this.BookmarksManager.RecreateBookmarksFromPersistableInfo(info);
                }
            }

            if (StorageOptions.AutoIncludeInGitignore && Helpers.PathIsInGitRepository(solutionPath))
            {
                var gitignorePath = Path.Combine(
                    Helpers.GetGitRepositoryRoot(solutionPath),
                    ".gitignore");
                Helpers.AddFileToGitignore(gitignorePath, DataFilePath, createGitignoreFile: false);
            }

            return VSConstants.S_OK;
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
