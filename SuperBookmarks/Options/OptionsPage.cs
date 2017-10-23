using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using System.Drawing;

namespace Konamiman.SuperBookmarks
{
    [Guid("BC1182FD-1E47-4616-BFC8-062683FB207A")]
    public class OptionsPage : DialogPage
    {
        OptionsControl control;

        private bool deletingALineDeletesTheBookmark = true;
        private bool navigateInFolderIncludesSubfolders = false;
        private bool navigateInFolderIncludesSubfoldersChanged = false;
        private bool optionsChanged = false;
        private bool deletingALineDeletesTheBookmarkChanged = false;
        private bool glyphColorChanged = false;
        private bool showCommandsInTopLevelMenu = false;
        private Color glyphColor;
        private int[] customColors;

        public OptionsPage()
        {
            control = new OptionsControl();
            control.Options = this;
        }

        public bool DeletingALineDeletesTheBookmark
        {
            get
            {
                return deletingALineDeletesTheBookmark;
            }
            set
            {
                deletingALineDeletesTheBookmark = value;
                deletingALineDeletesTheBookmarkChanged = true;
                optionsChanged = true;
            }
        }
        
        public bool NavigateInFolderIncludesSubfolders
        {
            get
            {
                return navigateInFolderIncludesSubfolders;
            }
            set
            {
                navigateInFolderIncludesSubfolders = value;
                navigateInFolderIncludesSubfoldersChanged = true;
                optionsChanged = true;
            }
        }

        public bool ShowCommandsInTopLevelMenu
        {
            get
            {
                return showCommandsInTopLevelMenu;
            }
            set
            {
                showCommandsInTopLevelMenu = value;
                optionsChanged = true;
            }
        }

        public Color GlyphColor
        {
            get
            {
                return glyphColor;
            }
            set
            {
                glyphColor = value;
                glyphColorChanged = true;
                optionsChanged = true;
            }
        }

        public int[] CustomColors
        {
            get
            {
                return customColors;
            }
            set
            {
                customColors = value;
                optionsChanged = true;
            }
        }

        public event EventHandler DeletingALineDeletesTheBookmarkChanged;
        public event EventHandler GlyphColorChanged;
        public event EventHandler NavigateInFolderIncludesSubfoldersChanged;
        public event EventHandler OptionsChanged;

        private bool applyClicked = false;
        private Color initialGlyphColor;

        protected override void OnApply(PageApplyEventArgs e)
        {
            applyClicked = true;

            if(deletingALineDeletesTheBookmarkChanged)
                DeletingALineDeletesTheBookmarkChanged?.Invoke(this, EventArgs.Empty);

            if(glyphColorChanged)
                GlyphColorChanged?.Invoke(this, EventArgs.Empty);

            if(navigateInFolderIncludesSubfoldersChanged)
                NavigateInFolderIncludesSubfoldersChanged?.Invoke(this, EventArgs.Empty);

            if(optionsChanged)
                OptionsChanged?.Invoke(this, EventArgs.Empty);

            //SetAsUnchanged(); --> OnClosed will take care of that
            base.OnApply(e);
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            initialGlyphColor = GlyphColor;
            control.Initialize();
            SetAsUnchanged();
            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!applyClicked)
                glyphColor = initialGlyphColor;

            SetAsUnchanged();
            base.OnClosed(e);
        }

        private void SetAsUnchanged()
        {
            optionsChanged = false;
            deletingALineDeletesTheBookmarkChanged = false;
            navigateInFolderIncludesSubfoldersChanged = false;
            glyphColorChanged = false;
            applyClicked = false;
        }

        protected override IWin32Window Window => control;
    }
}
