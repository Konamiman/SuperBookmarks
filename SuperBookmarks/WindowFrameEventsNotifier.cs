using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Konamiman.SuperBookmarks
{
    interface IWindowFrameEventsNotifier
    {
        IVsWindowFrame Frame { get; }

        event EventHandler FrameClosed;
    }

    class WindowFrameEventsNotifier : IWindowFrameEventsNotifier, IVsWindowFrameNotify, IVsWindowFrameNotify3
    {
        public WindowFrameEventsNotifier(IVsWindowFrame frame)
        {
            this.Frame = frame;
            (frame as IVsWindowFrame2)?.Advise(this, out adviseCookie);
        }

        private uint adviseCookie;

        public IVsWindowFrame Frame { get; }

        public event EventHandler FrameClosed;
        
        public int OnClose(ref uint pgrfSaveOptions)
        {
            (Frame as IVsWindowFrame2)?.Unadvise(adviseCookie);
            FrameClosed?.Invoke(this, EventArgs.Empty);

            return VSConstants.S_OK;
        }

        #region Unused members

        public int OnShow(int fShow)
        {
            return VSConstants.S_OK;
        }

        public int OnMove()
        {
            return VSConstants.S_OK;
        }

        public int OnSize()
        {
            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable)
        {
            return VSConstants.S_OK;
        }

        public int OnMove(int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
