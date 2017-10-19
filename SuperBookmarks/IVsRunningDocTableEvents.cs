using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        private Dictionary<IVsWindowFrame, string> openTextDocuments = null;

        private int _countOfOpenDocuments = 0;
        private int CountOfOpenDocuments
        {
            get
            {
                return _countOfOpenDocuments;
            }
            set
            {
                _countOfOpenDocuments = value;
                ThereAreOpenDocuments = CountOfOpenDocuments > 0;
            }
        }

        private void InvalidateCountOfOpenDocuments()
        {
            CountOfOpenDocuments = 0;
        }

        private List<string> initialListOfOpenDocuments;
        private void InitializeRunningDocumentsInfo()
        {
            const int reasonableMaxOpenDocuments = 10000;

            openTextDocuments = new Dictionary<IVsWindowFrame, string>();
            runningDocumentTable.GetRunningDocumentsEnum(out var rdEnum);
            var docCookies = new uint[reasonableMaxOpenDocuments];
            rdEnum.Reset();
            rdEnum.Next(reasonableMaxOpenDocuments, docCookies, out var fetched);

            initialListOfOpenDocuments = new List<string>();
            var tempCountOfOpenDocuments = 0;
            foreach (var docCookie in docCookies.Take((int)fetched))
            {
                var path = GetPathOfRunningDocument(docCookie, false, out var dummyProjectRootFolder);
                if (path == null)
                    continue;
                var docIsOpenInTextView = DocIsOpenInTextView(path, out var windowFrameForTextView);
                if (docIsOpenInTextView)
                {
                    openTextDocuments.Add(windowFrameForTextView, path);
                    BookmarksManager.OnTextDocumentOpen(path, windowFrameForTextView);
                    tempCountOfOpenDocuments++;
                    initialListOfOpenDocuments.Add(path);
                }

                //We need to check this because .sln and .*proj files count as running documents
                //(but are not considered open documents, fortunately)
                else if (DocIsOpenInAnyView(path))
                {
                    tempCountOfOpenDocuments++;
                    initialListOfOpenDocuments.Add(path);
                }
            }

            CountOfOpenDocuments = tempCountOfOpenDocuments;
            ThereAreOpenTextDocuments = openTextDocuments.Count > 0;
        }

        public IVsWindowFrame CurrentWindowFrame { get; private set; }
        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            //Need to check count==0 because sometimes in the first initialization
            //all the documents are reported as closed if they were open
            //together with the solution
            if (openTextDocuments == null || openTextDocuments.Count == 0)
                InitializeRunningDocumentsInfo();

            CurrentWindowFrame = pFrame;
            var path = GetPathOfRunningDocument(docCookie, true, out var docProjectFolder);
            if (path == null)
                return VSConstants.S_OK;

            var docIsOpenInTextView = DocIsOpenInTextView(path, out var windowFrameForTextView);

            ActiveDocumentIsText = docIsOpenInTextView && pFrame == windowFrameForTextView;
            
            if (ActiveDocumentIsText)
            {
                ThereAreOpenTextDocuments = true;
                if (!openTextDocuments.ContainsKey(pFrame))
                    openTextDocuments.Add(pFrame, path);
                if(fFirstShow != 0)
                    BookmarksManager.OnTextDocumentOpen(path, windowFrameForTextView);
            }

            //We need this extra check to prevent counting twice
            //the documents that were open together with the solution
            if (fFirstShow != 0)
            {
                if (initialListOfOpenDocuments.Contains(path))
                    initialListOfOpenDocuments.Remove(path);
                else
                    CountOfOpenDocuments++;
            }

            ActiveDocumentIsInProject = docProjectFolder != null;
            BookmarksManager.OnCurrentDocumentChanged(path, docProjectFolder);

            return VSConstants.S_OK;
        }

        private Dictionary<string, string> properlyCasedFilenamesCache = 
            new Dictionary<string, string>();
        private string GetPathOfRunningDocument(uint docCookie, bool getProjectRootFolder, out string projectRootFolder)
        {
            runningDocumentTable.GetDocumentInfo(docCookie,
                out var flags, out var dummyReadLocks,
                out var dummyEditLocks, out string path,
                out var dummyHierarchy,
                out var dummyItemId, out var dummyData);

            projectRootFolder = null;
            if (((int)flags & ((int)VsRdtFlags.VirtualDocument | (int)VsRdtFlags.ProjSlnDocument)) != 0)
                return null;

            if (getProjectRootFolder)
            {
                var projectHierarchy = VsShellUtilities.GetProject(this, path);
                if (projectHierarchy != null)
                {
                    projectHierarchy.GetCanonicalName((uint) VSConstants.VSITEMID.Root, out string canonicalName);
                    if (canonicalName != null &&
                        //For projectless solution items GetCanonicalName returns a guid
                        !Guid.TryParse(canonicalName, out var dummyGuid))
                    {
                        projectRootFolder = Helpers.GetProperlyCasedPath(canonicalName);
                    }
                }
            }

            //We need this because sometimes GetDocumentInfo returns the path lowercased
            return Helpers.GetProperlyCasedPath(path);
        }

        private bool DocIsOpenInTextView(string path, out IVsWindowFrame windowFrame)
        {
            return DocIsOpenInLogicalView(path, VSConstants.LOGVIEWID_Code, out windowFrame) ||
                   DocIsOpenInLogicalView(path, VSConstants.LOGVIEWID_TextView, out windowFrame);
        }

        private bool DocIsOpenInLogicalView(string path, Guid logicalView, out IVsWindowFrame windowFrame)
        {
            return VsShellUtilities.IsDocumentOpen(
                this,
                path,
                VSConstants.LOGVIEWID_TextView,
                out var dummyHierarchy2, out var dummyItemId2,
                out windowFrame);
        }

        private bool DocIsOpenInAnyView(string path)
        {
            return VsShellUtilities.IsDocumentOpen(
                this,
                path,
                Guid.Empty,
                out var dummyHierarchy2, out var dummyItemId2, out var dummyWindowFrame);
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            CountOfOpenDocuments--;

            if (!openTextDocuments.ContainsKey(pFrame))
                return VSConstants.S_OK;

            BookmarksManager.OnTextDocumentClosed(openTextDocuments[pFrame]);

            openTextDocuments.Remove(pFrame);

            ThereAreOpenTextDocuments = openTextDocuments.Count > 0;
            if (!ThereAreOpenTextDocuments)
            {
                ActiveDocumentIsText = false;
                ActiveDocumentIsInProject = false;
            }

            return VSConstants.S_OK;
        }

        #region Unused members

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
