using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public OptionsPage Options { get; private set; }

        private WritableSettingsStore settingsStore;

        private const int intFalse = 0;
        private const int intTrue = 1;

        private void InitializeOptionsStorage()
        {
            var settingsManager = new ShellSettingsManager(this);
            settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!settingsStore.CollectionExists(SettingsStoreName))
                settingsStore.CreateCollection(SettingsStoreName);

            Options = (OptionsPage)GetDialogPage(typeof(OptionsPage));
            SetOptionsFromStorage();
            Options.OptionsChanged += (sender, args) => SaveOptionsToStorage();
        }

        private void SetOptionsFromStorage()
        {
            Options.DeletingALineDeletesTheBookmark =
                settingsStore.GetInt32(SettingsStoreName, "DeletingALineDeletesTheBookmark", intTrue) == intTrue;
        }

        private void SaveOptionsToStorage()
        {
            settingsStore.SetInt32(SettingsStoreName,
                "DeletingALineDeletesTheBookmark",
                Options.DeletingALineDeletesTheBookmark ? intTrue : intFalse);
        }
    }
}
