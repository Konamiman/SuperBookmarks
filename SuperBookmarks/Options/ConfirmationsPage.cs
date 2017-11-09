using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Konamiman.SuperBookmarks.Options
{
    [Guid("2A147C3B-3B79-4746-BF40-F97AA8569BC4")]
    public class ConfirmationsPage : OptionsPageBase
    {
        ConfirmationOptionsControl control;

        public ConfirmationsPage()
        {
            control = new ConfirmationOptionsControl();
            control.Options = this;
        }

        public bool DelAllInDocumentRequiresConfirmation { get; set; }

        public bool DelAllInOpenDocumentsRequiresConfirmation { get; set; }

        public bool DelAllInFolderRequiresConfirmation { get; set; }

        public bool DelAllInProjectRequiresConfirmation { get; set; }

        public bool DelAllInSolutionRequiresConfirmation { get; set; }

        public bool ReplacingImportRequiresConfirmation { get; set; }

        public bool ReplacingLoadRequiresConfirmation { get; set; }

        protected override void OnActivate(CancelEventArgs e)
        {
            Helpers.SafeInvoke(control.Initialize);
            base.OnActivate(e);
        }

        protected override IWin32Window Window => control;

        public override void ResetSettings()
        {
            DelAllInDocumentRequiresConfirmation = true;
            DelAllInOpenDocumentsRequiresConfirmation = true;
            DelAllInFolderRequiresConfirmation = true;
            DelAllInProjectRequiresConfirmation = true;
            ReplacingImportRequiresConfirmation = true;
            ReplacingLoadRequiresConfirmation = true;
        }

        public override void LoadSettingsFromStorage()
        {
            bool Convert(char value) => value == '1';

            var settingValue = LoadStringProperty("ConfirmationOptions", "1111111").PadRight(7, '1');

            DelAllInDocumentRequiresConfirmation = Convert(settingValue[0]);
            DelAllInOpenDocumentsRequiresConfirmation = Convert(settingValue[1]);
            DelAllInFolderRequiresConfirmation = Convert(settingValue[2]);
            DelAllInProjectRequiresConfirmation = Convert(settingValue[3]);
            DelAllInSolutionRequiresConfirmation = Convert(settingValue[4]);
            ReplacingImportRequiresConfirmation = Convert(settingValue[5]);
            ReplacingLoadRequiresConfirmation = Convert(settingValue[6]);
        }

        public override void SaveSettingsToStorage()
        {
            string Convert(bool value) => value ? "1" : "0";

            var settingValue = Convert(DelAllInDocumentRequiresConfirmation) +
                   Convert(DelAllInOpenDocumentsRequiresConfirmation) +
                   Convert(DelAllInFolderRequiresConfirmation) +
                   Convert(DelAllInProjectRequiresConfirmation) +
                   Convert(DelAllInSolutionRequiresConfirmation) +
                   Convert(ReplacingImportRequiresConfirmation) +
                   Convert(ReplacingLoadRequiresConfirmation);

            SaveProperty("ConfirmationOptions", settingValue);
        }

        public bool ShouldConfirmForDeleteAllIn(BookmarkActionTarget target)
        {
            switch (target)
            {
                case BookmarkActionTarget.Document:
                    return DelAllInDocumentRequiresConfirmation;
                case BookmarkActionTarget.OpenDocuments:
                    return DelAllInOpenDocumentsRequiresConfirmation;
                case BookmarkActionTarget.Folder:
                case BookmarkActionTarget.FolderAndSubfolders:
                    return DelAllInFolderRequiresConfirmation;
                case BookmarkActionTarget.Project:
                    return DelAllInProjectRequiresConfirmation;
                case BookmarkActionTarget.Solution:
                    return DelAllInSolutionRequiresConfirmation;
                default:
                    return true;
            }
        }
    }
}
