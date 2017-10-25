using System;
using System.Drawing;
using System.Linq;
using Konamiman.SuperBookmarks.Options;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public OptionsPage Options { get; private set; }
        public StorageOptionsPage StorageOptions { get; private set; }
        public ConfirmationsPage ConfirmationOptions { get; private set; }

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
            StorageOptions.OptionsChanged += (sender, args) => SavePersistenceOptionsToStorage();

            ConfirmationOptions = (ConfirmationsPage) GetDialogPage(typeof(ConfirmationsPage));
            SetConfirmationOptionsFromStorage();
            ConfirmationOptions.OptionsChanged += (sender, args) => SaveConfirmationOptionsToStorage();
        }

        private void SetOptionsFromStorage()
        {
            Options.DeletingALineDeletesTheBookmark =
                settingsStore.GetInt32(SettingsStoreName, "DeletingALineDeletesTheBookmark", intTrue) == intTrue;

            Options.ShowCommandsInTopLevelMenu =
                settingsStore.GetInt32(SettingsStoreName, "ShowCommandsInTopLevelMenu", intFalse) == intTrue;

            Options.NavigateInFolderIncludesSubfolders =
                settingsStore.GetInt32(SettingsStoreName, "NavigateInFolderIncludesSubfolders", intFalse) == intTrue;

            Options.DeleteAllInFolderIncludesSubfolders =
                settingsStore.GetInt32(SettingsStoreName, "DeleteAllInFolderIncludesSubfolders", intFalse) == intTrue;

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
                "NavigateInFolderIncludesSubfolders",
                Options.NavigateInFolderIncludesSubfolders ? intTrue : intFalse);

            settingsStore.SetInt32(SettingsStoreName,
                "DeleteAllInFolderIncludesSubfolders",
                Options.DeleteAllInFolderIncludesSubfolders ? intTrue : intFalse);

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

        private void SavePersistenceOptionsToStorage()
        {
            settingsStore.SetInt32(SettingsStoreName,
                "SaveBookmarksToOwnFile",
                StorageOptions.SaveBookmarksToOwnFile ? intTrue : intFalse);

            settingsStore.SetInt32(SettingsStoreName,
                "AutoIncludeInGitignore",
                StorageOptions.AutoIncludeInGitignore ? intTrue : intFalse);
        }

        private void SetConfirmationOptionsFromStorage()
        {
            ConfirmationOptions.Deserialize(
                settingsStore.GetString(SettingsStoreName, "ConfirmationOptions", "11111"));
        }

        private void SaveConfirmationOptionsToStorage()
        {
            settingsStore.SetString(SettingsStoreName, "ConfirmationOptions", ConfirmationOptions.Serialize());
        }
    }
}
