using System;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public OptionsPage Options { get; private set; }
        public StorageOptionsPage StorageOptions { get; private set; }

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

            StorageOptions = (StorageOptionsPage) GetDialogPage(typeof(StorageOptionsPage));
            SetPersistenceOptionsFromStorage();
            StorageOptions.OptionsChanged += SavePersistenceOptionsToStorage;
        }

        private void SetOptionsFromStorage()
        {
            Options.DeletingALineDeletesTheBookmark =
                settingsStore.GetInt32(SettingsStoreName, "DeletingALineDeletesTheBookmark", intTrue) == intTrue;

            Options.ShowCommandsInTopLevelMenu =
                settingsStore.GetInt32(SettingsStoreName, "ShowCommandsInTopLevelMenu", intFalse) == intTrue;

            var glypColorRgb =
                settingsStore.GetInt32(SettingsStoreName, "GlyphColor", BookmarkGlyphFactory.DefaultColor.ToArgb());
            Options.GlyphColor = Color.FromArgb(glypColorRgb);
            BookmarkGlyphFactory.SetGlyphColor(Options.GlyphColor);

            var customColorsRgbs =
                settingsStore.GetString(SettingsStoreName, "CustomColors", null);
            Options.CustomColors =
                customColorsRgbs?.Split(',').Select(rgb => int.Parse(rgb)).ToArray();
        }

        private void SaveOptionsToStorage()
        {
            settingsStore.SetInt32(SettingsStoreName,
                "DeletingALineDeletesTheBookmark",
                Options.DeletingALineDeletesTheBookmark ? intTrue : intFalse);

            settingsStore.SetInt32(SettingsStoreName,
                "ShowCommandsInTopLevelMenu",
                Options.ShowCommandsInTopLevelMenu ? intTrue : intFalse);

            settingsStore.SetInt32(SettingsStoreName,
                "GlyphColor",
                Options.GlyphColor.ToArgb());

            if (Options.CustomColors != null)
            {
                settingsStore.SetString(SettingsStoreName,
                    "CustomColors",
                    string.Join(",", Options.CustomColors.Select(rgb => rgb.ToString())));
            }
        }

        private void SetPersistenceOptionsFromStorage()
        {
            StorageOptions.SaveBookmarksToOwnFile =
                settingsStore.GetInt32(SettingsStoreName, "SaveBookmarksToOwnFile", intFalse) == intTrue;

            StorageOptions.AutoIncludeInGitignore =
                settingsStore.GetInt32(SettingsStoreName, "AutoIncludeInGitignore", intFalse) == intTrue;
        }

        private void SavePersistenceOptionsToStorage(object sender, EventArgs eventArgs)
        {
            settingsStore.SetInt32(SettingsStoreName,
                "SaveBookmarksToOwnFile",
                StorageOptions.SaveBookmarksToOwnFile ? intTrue : intFalse);

            settingsStore.SetInt32(SettingsStoreName,
                "AutoIncludeInGitignore",
                StorageOptions.AutoIncludeInGitignore ? intTrue : intFalse);
        }
    }
}
