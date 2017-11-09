using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace Konamiman.SuperBookmarks
{
    [Guid("BC1182FD-1E47-4616-BFC8-062683FB207A")]
    public class GeneralOptionsPage : OptionsPageBase
    {
        GeneralOptionsControl control;

        private bool deletingALineDeletesTheBookmarkChanged = false;
        private bool glyphColorChanged = false;
        private bool showMenuOptionChanged = false;

        public GeneralOptionsPage()
        {
            control = new GeneralOptionsControl();
            control.Options = this;
        }

        private bool deletingALineDeletesTheBookmark = false;
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
            }
        }
        
        public bool NavigateInFolderIncludesSubfolders { get; set; }

        public bool DeleteAllInFolderIncludesSubfolders { get; set; }

        public bool MergeWhenImporting { get; set; }

        private ShowMenuOption showMenuOption;
        public ShowMenuOption ShowMenuOption
        {
            get
            {
                return showMenuOption;
            }
            set
            {
                showMenuOption = value;
                showMenuOptionChanged = true;
            }
        }

        private Color glyphColor;
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
            }
        }

        public int[] CustomColors { get; set; }

        public event EventHandler DeletingALineDeletesTheBookmarkChanged;
        public event EventHandler GlyphColorChanged;
        public event EventHandler ShowMenuOptionChanged;

        protected override void OnApply(PageApplyEventArgs e) =>
            Helpers.SafeInvoke(() => _OnApply(e));

        private void _OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                if (deletingALineDeletesTheBookmarkChanged)
                    DeletingALineDeletesTheBookmarkChanged?.Invoke(this, EventArgs.Empty);

                if (glyphColorChanged)
                    GlyphColorChanged?.Invoke(this, EventArgs.Empty);

                if (showMenuOptionChanged)
                    ShowMenuOptionChanged?.Invoke(this, EventArgs.Empty);
            }

            deletingALineDeletesTheBookmarkChanged = false;
            glyphColorChanged = false;
            showMenuOptionChanged = false;

            base.OnApply(e);
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            control.Initialize();
            base.OnActivate(e);
        }

        public override void ResetSettings()
        {
            DeletingALineDeletesTheBookmark = true;
            NavigateInFolderIncludesSubfolders = false;
            DeleteAllInFolderIncludesSubfolders = false;
            MergeWhenImporting = false;
            ShowMenuOption = ShowMenuOption.WithTitleSuperBookmarks;
            GlyphColor = BookmarkGlyphFactory.DefaultColor;
            CustomColors = new int[0];
        }

        public override void LoadSettingsFromStorage()
        {
            DeletingALineDeletesTheBookmark = LoadBooleanProperty("DeletingALineDeletesTheBookmark", true);
            ShowMenuOption = (ShowMenuOption)LoadIntProperty("ShowMenuOption", (int)ShowMenuOption.WithTitleSuperBookmarks);
            NavigateInFolderIncludesSubfolders = LoadBooleanProperty("NavigateInFolderIncludesSubfolders", true);
            DeleteAllInFolderIncludesSubfolders = LoadBooleanProperty("DeleteAllInFolderIncludesSubfolders", false);
            MergeWhenImporting = LoadBooleanProperty("MergeWhenImporting", false);

            var glypColorRgb = LoadIntProperty("GlyphColor", BookmarkGlyphFactory.DefaultColor.ToArgb());
            GlyphColor = Color.FromArgb(glypColorRgb);
            BookmarkGlyphFactory.SetGlyphColor(GlyphColor);

            var customColorsRgbs = LoadStringProperty("CustomColors", null);
            CustomColors = customColorsRgbs?.Split(',').Select(rgb => int.Parse(rgb)).ToArray();
        }

        public override void SaveSettingsToStorage()
        {
            SaveProperty("DeletingALineDeletesTheBookmark", DeletingALineDeletesTheBookmark);
            SaveProperty("NavigateInFolderIncludesSubfolders", NavigateInFolderIncludesSubfolders);
            SaveProperty("DeleteAllInFolderIncludesSubfolders", DeleteAllInFolderIncludesSubfolders);
            SaveProperty("ShowMenuOption", (int)ShowMenuOption);
            SaveProperty("GlyphColor", GlyphColor.ToArgb());
            SaveProperty("MergeWhenImporting", MergeWhenImporting);

            if (CustomColors != null)
            {
                SaveProperty("CustomColors",
                    string.Join(",", CustomColors.Select(rgb => rgb.ToString())));
            }
        }

        protected override IWin32Window Window => control;
    }
}
