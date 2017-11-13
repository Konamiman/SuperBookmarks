using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Diagnostics;

namespace Konamiman.SuperBookmarks
{
    /*
     Managing the running documents table is somewhat tricky.
     Here are a few things to consider:
     
     - Each physical document appears only once in the table, with a unique doc cookie;
       but some types of documents can be open simultaneously in two window frames,
       one for the code view and one for the design view.
       
     - Documents that were open together with the solution (because they were open
       when the solution was last closed) don't trigger OnBeforeDocumentWindowShow
       until they are first activated. That's why we do an initial manual
       filling of the list of open documents.

     - Also for documents that were open together with the solution: if they are
       never given focus after the solution has loaded, they won't trigger 
       OnAfterDocumentWindowHide when they are closed. That's why we use
       IWindowFrameEventsNotifier instead to detect closures - 
       more complex, but bulletproof.
       
     - GetRunningDocumentsEnum won't give reliable values if called right after
       the solution finishes loading, because of the async project loading thing
       (presumably). So we invoke it in OnBeforeDocumentWindowShow instead -
       and yet, the first time it won't give reliable values (it will report
       only the active document, and with no proper window frame available yet).
       That's why we invoke it on each OnBeforeDocumentWindowShow if there are
       no registered documents. Less than ideal but it works.
     */

    public partial class SuperBookmarksPackage
    {
        //We keep one of these for each window frame open
        //(NOT for each physical document open),
        //so don't rely on paths being unique
        class OpenDocumentFrameInfo
        {
            public OpenDocumentFrameInfo(string path, string projectRoot, IVsWindowFrame frame, bool isTextView)
            {
                Path = path;
                ProjectRoot = projectRoot;
                IsTextView = isTextView;
                FrameEventsNotifier = new WindowFrameEventsNotifier(frame);
            }

            public string Path { get; }
            public string ProjectRoot { get; }
            public bool IsTextView { get; }
            public IWindowFrameEventsNotifier FrameEventsNotifier { get; }
        }

        private Dictionary<IVsWindowFrame, OpenDocumentFrameInfo> openDocumentFrames =
            new Dictionary<IVsWindowFrame, OpenDocumentFrameInfo>();

        public IVsWindowFrame CurrentWindowFrame { get; private set; }

        private void TearDownRunningDocumentsInfo()
        {
            foreach (var frame in openDocumentFrames.Keys)
            {
                openDocumentFrames[frame].FrameEventsNotifier.FrameClosed -= OnFrameClosed;
            }

            openDocumentFrames.Clear();
            UpdateOpenDocumentsState();
        }

        private void InitializeRunningDocumentsInfo()
        {
            const int reasonableMaxOpenDocuments = 10000;

            runningDocumentTable.GetRunningDocumentsEnum(out var rdEnum);
            var docCookies = new uint[reasonableMaxOpenDocuments];
            rdEnum.Reset();
            rdEnum.Next(reasonableMaxOpenDocuments, docCookies, out var fetched);

            foreach (var docCookie in docCookies.Take((int)fetched))
                RegisterOpenDocument(docCookie);
        }

        private void RegisterOpenDocument(uint docCookie, IVsWindowFrame frame = null)
        {
            if (frame != null && openDocumentFrames.ContainsKey(frame))
                return;

            (string documentPath, string projectRoot) = GetRunningDocumentPaths(docCookie);
            if (documentPath == null)
                return;

            void RegisterOpenFrame(IVsWindowFrame currentFrame, bool isTextDocument)
            {
                var docInfo = new OpenDocumentFrameInfo(documentPath, projectRoot, currentFrame, isTextDocument);
                if(!openDocumentFrames.ContainsKey(currentFrame))
                    openDocumentFrames.Add(currentFrame, docInfo);

                docInfo.FrameEventsNotifier.FrameClosed += OnFrameClosed;

                if(isTextDocument)
                    BookmarksManager.OnTextDocumentOpen(documentPath);

                Helpers.Debug($"Frame Registered: {Path.GetFileName(documentPath)}, isText: {isTextDocument}, total open: {openDocumentFrames.Count}");
            }

            var frameForTextView = GetFrameForTextView(documentPath);
            var frameForDesignView = GetFrameForDesignerView(documentPath);

            if (frame == null)
            {
                if (frameForTextView != null)
                    RegisterOpenFrame(frameForTextView, true);

                if (frameForDesignView != null)
                    RegisterOpenFrame(frameForDesignView, false);
            }
            else
            {
                if(frameForTextView == frame)
                    RegisterOpenFrame(frame, true);
                else if(frameForDesignView == frame)
                    RegisterOpenFrame(frame, false);
            }

            UpdateOpenDocumentsState();
        }

        private void OnFrameClosed(object sender, EventArgs eventArgs) =>
            Helpers.SafeInvoke(() => _OnFrameClosed(sender));

        private void _OnFrameClosed(object sender)
        {
            var frame = ((IWindowFrameEventsNotifier)sender).Frame;
            if (!openDocumentFrames.ContainsKey(frame))
                return;

            var document = openDocumentFrames[frame];
            if(document.IsTextView)
                BookmarksManager.OnTextDocumentClosed(document.Path);

            document.FrameEventsNotifier.FrameClosed -= OnFrameClosed;

            openDocumentFrames.Remove(frame);
            UpdateOpenDocumentsState();

            Helpers.Debug($"Frame Closed: {Path.GetFileName(document.Path)}, isText: {document.IsTextView}, total open: {openDocumentFrames.Count}");
        }

        void UpdateOpenDocumentsState()
        {
            ThereAreOpenDocuments = openDocumentFrames.Count > 0;
            ThereAreOpenTextDocuments = openDocumentFrames.Values.Any(doc => doc.IsTextView);
            if (!ThereAreOpenDocuments)
            {
                ActiveDocumentIsText = false;
                ActiveDocumentIsInProject = false;
                CurrentWindowFrame = null;
            }
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame) =>
            Helpers.SafeInvoke(() => _OnBeforeDocumentWindowShow(docCookie, fFirstShow, pFrame));

        private int _OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            if(openDocumentFrames.Count == 0)
                InitializeRunningDocumentsInfo();

            var isFirstShow = fFirstShow != 0;
            if (isFirstShow && !openDocumentFrames.ContainsKey(pFrame))
                RegisterOpenDocument(docCookie, pFrame);

            if (!openDocumentFrames.ContainsKey(pFrame))
                return VSConstants.S_OK;

            if (!isFirstShow && CurrentWindowFrame == pFrame)
                return VSConstants.S_OK;

            CurrentWindowFrame = pFrame;

            var document = openDocumentFrames[pFrame];
            BookmarksManager.OnCurrentDocumentChanged(document.Path, document.ProjectRoot);
            ActiveDocumentIsText = document.IsTextView;
            ActiveDocumentIsInProject = document.ProjectRoot != null;

            Helpers.Debug($"Frame Shown: {Path.GetFileName(document.Path)}, isText: {document.IsTextView}, total open: {openDocumentFrames.Count}");

            return VSConstants.S_OK;
        }

        private (string documentPath, string projectRootPath) GetRunningDocumentPaths(uint docCookie)
        {
            runningDocumentTable.GetDocumentInfo(docCookie,
                out var flags, out var dummyReadLocks,
                out var dummyEditLocks, out string documentPath,
                out var dummyHierarchy,
                out var dummyItemId, out var dummyData);

            const int excludeFlags =
                (int)VsRdtFlags.VirtualDocument |
                (int)VsRdtFlags.ProjSlnDocument |
                (int)VsRdtFlags.DontSaveAs |
                (int)VsRdtFlags.DontAddToMRU;

            if (((int)flags & excludeFlags) != 0)
                return (null, null);

            string projectRootFolder = null;
            if (documentPath.StartsWith(CurrentSolutionPath, StringComparison.OrdinalIgnoreCase))
            {
                var projectHierarchy = VsShellUtilities.GetProject(this, documentPath);
                if (projectHierarchy != null)
                {
                    projectHierarchy.GetCanonicalName((uint)VSConstants.VSITEMID.Root, out string canonicalName);
                    if (canonicalName != null &&
                        //For projectless solution items GetCanonicalName returns a guid
                        !Guid.TryParse(canonicalName, out var dummyGuid))
                    {
                        projectRootFolder = Helpers.GetProperlyCasedPath(canonicalName);
                    }
                }
            }

            //We need this because sometimes GetDocumentInfo returns the path lowercased
            documentPath = Helpers.GetProperlyCasedPath(documentPath);

            return (documentPath, projectRootFolder);
        }

        private IVsWindowFrame GetFrameForTextView(string path)
        {
            return GetFrameForLogicalView(path, VSConstants.LOGVIEWID_Code) ??
                   GetFrameForLogicalView(path, VSConstants.LOGVIEWID_TextView);
        }

        private IVsWindowFrame GetFrameForDesignerView(string path)
        {
            return GetFrameForLogicalView(path, VSConstants.LOGVIEWID_Designer);
        }

        private IVsWindowFrame GetFrameForLogicalView(string path, Guid logicalView)
        {
            return VsShellUtilities.IsDocumentOpen(
                this,
                path,
                logicalView,
                out var dummyHierarchy2, out var dummyItemId2,
                out var windowFrame) ? windowFrame : null;
        }

        #region Unused members

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
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
