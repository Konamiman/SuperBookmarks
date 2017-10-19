using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public string DataFilePath =>
            Path.Combine(Path.GetDirectoryName(CurrentSolutionSuoPath), ".SuperBookmarks.dat");

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            SaveBookmarksToDatFile();
            this.BookmarksManager.ClearAllBookmarks();
            this.BookmarksManager.OnSolutionClosed();

            SolutionIsCurrentlyOpen = false;
            ActiveDocumentIsText = false;
            ThereAreOpenTextDocuments = false;
            ActiveDocumentIsInProject = false;

            openTextDocuments = null;
            InvalidateCountOfOpenDocuments();
            Helpers.ClearProperlyCasedPathsCache();

            return VSConstants.S_OK;
        }

        private void SaveBookmarksToDatFile()
        {
            if (StorageOptions.SaveBookmarksToOwnFile)
            {
                var info = this.BookmarksManager.GetPersistableInfo();
                using (var stream = File.Create(DataFilePath))
                    info.SerializeTo(stream);
            }
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
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

        private void LoadBookmarksFromDatFile()
        {
            if (StorageOptions.SaveBookmarksToOwnFile)
            {
                if (File.Exists(DataFilePath))
                {
                    using (var stream = File.OpenRead(DataFilePath))
                    {
                        var info = PersistableBookmarksInfo.DeserializeFrom(stream);
                        this.BookmarksManager.RecreateBookmarksFromPersistableInfo(info);
                    }
                }
                else
                {
                    this.BookmarksManager.ClearAllBookmarks();
                }
            }
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
            var itemid = VSConstants.VSITEMID_ROOT;

            object objProj;
            pRealHierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_ProjectDir, out objProj);

            //var projectItem = objProj as EnvDTE.ProjectItem;
            //// ... or ...
            //var project = objProj as EnvDTE.Project;

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
