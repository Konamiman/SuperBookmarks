using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace Konamiman.SuperBookmarks
{
    public abstract class OptionsPageBase : DialogPage
    {
        private const int intFalse = 0;
        private const int intTrue = 1;

        private WritableSettingsStore settingsStore;
        public const string SettingsStoreName = "Konamiman.SuperBookmarks";

        public static WritableSettingsStore GetSettingsStore()
        {
            var settingsManager = new ShellSettingsManager(SuperBookmarksPackage.Instance);
            return settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public OptionsPageBase()
        {
            settingsStore = GetSettingsStore();
            if (!settingsStore.CollectionExists(SettingsStoreName))
                settingsStore.CreateCollection(SettingsStoreName);
        }

        protected bool LoadBooleanProperty(string name, bool defaultValue)
        {
            var value = settingsStore.GetInt32(SettingsStoreName, name, defaultValue ? intTrue : intFalse);
            return value == intTrue;
        }

        protected void SaveProperty(string name, bool value)
        {
            settingsStore.SetInt32(SettingsStoreName, name, value ? intTrue : intFalse);
        }

        protected int LoadIntProperty(string name, int defaultValue)
        {
            return settingsStore.GetInt32(SettingsStoreName, name, defaultValue);
        }

        protected void SaveProperty(string name, int value)
        {
            settingsStore.SetInt32(SettingsStoreName, name, value);
        }

        protected string LoadStringProperty(string name, string defaultValue)
        {
            return settingsStore.GetString(SettingsStoreName, name, defaultValue);
        }

        protected void SaveProperty(string name, string value)
        {
            settingsStore.SetString(SettingsStoreName, name, value);
        }
    }
}
