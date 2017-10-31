using Konamiman.SuperBookmarks.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public GeneralOptionsPage Options { get; private set; }
        public StorageOptionsPage StorageOptions { get; private set; }
        public ConfirmationsPage ConfirmationOptions { get; private set; }
        public DebugPage DebugOptions { get; private set; }

        private void InitializeOptionsStorage()
        {
            Options = (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));
            StorageOptions = (StorageOptionsPage)GetDialogPage(typeof(StorageOptionsPage));
            ConfirmationOptions = (ConfirmationsPage)GetDialogPage(typeof(ConfirmationsPage));
            DebugOptions = (DebugPage)GetDialogPage(typeof(DebugPage));

            Options.GlyphColorChanged += (sender, args) =>
            {
                BookmarkGlyphFactory.SetGlyphColor(Options.GlyphColor);

                //Force redraw of currently visible bookmarks
                //(there must be a better way to do this...)
                if (CurrentWindowFrame?.IsVisible() == VSConstants.S_OK)
                {
                    CurrentWindowFrame.Hide();
                    CurrentWindowFrame.Show();
                }
            };

            Options.ShowMenuOptionChanged += (sender, args) => UpdateMenuVisibilityAndText();
        }

        WritableSettingsStore settingsStore = null;
        WritableSettingsStore SettingsStore =>
            settingsStore ??
            (settingsStore = OptionsPageBase.GetSettingsStore());

        string cachedLastUsedImportExportFolder = null;
        public string GetLastUsedExportImportFolder()
        {
            if(cachedLastUsedImportExportFolder == null)
            {
                cachedLastUsedImportExportFolder = 
                    settingsStore.GetString(SettingsStoreName, "LastUsedExportImportFolder", "")
                    .WithTrailingDirectorySeparator();
            }
            return cachedLastUsedImportExportFolder;
        }

        public void SetLastUsedExportImportFolder(string value)
        {
            value = value.WithTrailingDirectorySeparator();
            settingsStore.SetString(SettingsStoreName, "LastUsedExportImportFolder", value);
        }
    }
}
