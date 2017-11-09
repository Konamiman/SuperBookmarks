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
            => Helpers.SafeInvoke(_GetSettingsStore);

        private static WritableSettingsStore _GetSettingsStore()
        {
            var settingsManager = new ShellSettingsManager(SuperBookmarksPackage.Instance);
            return settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public OptionsPageBase() =>
            Helpers.SafeInvoke(Constructor);

        private void Constructor()
        {
            settingsStore = GetSettingsStore();
            if (!settingsStore.CollectionExists(SettingsStoreName))
                settingsStore.CreateCollection(SettingsStoreName);
        }

        protected bool LoadBooleanProperty(string name, bool defaultValue) =>
            Helpers.SafeInvoke(() => _LoadBooleanProperty(name, defaultValue));

        private bool _LoadBooleanProperty(string name, bool defaultValue)
        {
            var value = settingsStore.GetInt32(SettingsStoreName, name, defaultValue ? intTrue : intFalse);
            return value == intTrue;
        }

        protected void SaveProperty(string name, bool value) =>
            Helpers.SafeInvoke(() => 
            settingsStore.SetInt32(SettingsStoreName, name, value ? intTrue : intFalse));

        protected int LoadIntProperty(string name, int defaultValue) =>
            Helpers.SafeInvoke(() =>
            settingsStore.GetInt32(SettingsStoreName, name, defaultValue), defaultValue);

        protected void SaveProperty(string name, int value) =>
            Helpers.SafeInvoke(() =>
            settingsStore.SetInt32(SettingsStoreName, name, value));

        protected string LoadStringProperty(string name, string defaultValue) =>
            Helpers.SafeInvoke(() =>
            settingsStore.GetString(SettingsStoreName, name, defaultValue), defaultValue);

        protected void SaveProperty(string name, string value) =>
            Helpers.SafeInvoke(() =>
            settingsStore.SetString(SettingsStoreName, name, value));
    }
}
