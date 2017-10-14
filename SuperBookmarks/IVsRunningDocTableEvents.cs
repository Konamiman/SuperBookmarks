using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        private Dictionary<IVsWindowFrame, string> _openTextDocuments = null;

        private Dictionary<IVsWindowFrame, string> OpenTextDocuments
        {
            get
            { 
                if(_openTextDocuments == null)
                    InitializeRunningDocumentsInfo();

                return _openTextDocuments;
            }
        }

        private void InitializeRunningDocumentsInfo()
        {
            const int reasonableMaxOpenDocuments = 10000;

            _openTextDocuments = new Dictionary<IVsWindowFrame, string>();
            var runnigDocsEnum = runningDocumentTable.GetRunningDocumentsEnum(out var rdEnum);
            var docCookies = new uint[reasonableMaxOpenDocuments];
            rdEnum.Reset();
            rdEnum.Next(reasonableMaxOpenDocuments, docCookies, out var fetched);

            _openTextDocuments.Clear();
            foreach (var docCookie in docCookies.Take((int)fetched))
            {
                var path = GetPathOfRunningDocument(docCookie);
                var docIsOpenInTextView = DocIsOpenInTextView(path, out var windowFrameForTextView);
                if (docIsOpenInTextView)
                {
                    _openTextDocuments.Add(windowFrameForTextView, path);
                    BookmarksManager.OnTextDocumentOpen(path, windowFrameForTextView);
                }
            }

            ThereAreOpenDocuments = _openTextDocuments.Count > 0;
        }

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

        public IVsWindowFrame CurrentWindowFrame { get; private set; }
        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            CurrentWindowFrame = pFrame;
            var path = GetPathOfRunningDocument(docCookie);
            var docIsOpenInTextView = DocIsOpenInTextView(path, out var windowFrameForTextView);

            ActiveDocumentIsText = docIsOpenInTextView && pFrame == windowFrameForTextView;

            if (ActiveDocumentIsText)
            {
                if (!OpenTextDocuments.ContainsKey(pFrame))
                {
                    OpenTextDocuments.Add(pFrame, path);
                    ThereAreOpenDocuments = true;
                    BookmarksManager.OnTextDocumentOpen(path, windowFrameForTextView);
                }
                BookmarksManager.OnCurrentDocumentChanged(path);
            }

            return VSConstants.S_OK;
        }

        private string GetPathOfRunningDocument(uint docCookie)
        {
            var x = runningDocumentTable.GetDocumentInfo(docCookie,
                out var dummyFlags, out var dummyReadLocks,
                out var dummyEditLocks, out string path,
                out var dummyHierarchy,
                out var dummyItemId, out var dummyData);

            return path;
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

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            if (!OpenTextDocuments.ContainsKey(pFrame))
                return VSConstants.S_OK;

            BookmarksManager.OnTextDocumentClosed(OpenTextDocuments[pFrame]);

            OpenTextDocuments.Remove(pFrame);

            ThereAreOpenDocuments = OpenTextDocuments.Count > 0;
            if (!ThereAreOpenDocuments)
                ActiveDocumentIsText = false;

            return VSConstants.S_OK;
        }
    }
}
