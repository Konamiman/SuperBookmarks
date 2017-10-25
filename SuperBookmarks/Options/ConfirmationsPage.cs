using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Konamiman.SuperBookmarks.Options
{
    [Guid("2A147C3B-3B79-4746-BF40-F97AA8569BC4")]
    public class ConfirmationsPage : DialogPage
    {
        ConfirmationOptionsControl control;

        private bool optionsChanged = false;

        public ConfirmationsPage()
        {
            control = new ConfirmationOptionsControl();
            control.Options = this;
        }

        private bool delAllInDocumentRequiresConfirmation;
        public bool DelAllInDocumentRequiresConfirmation
        {
            get
            {
                return delAllInDocumentRequiresConfirmation;
            }
            set
            {
                delAllInDocumentRequiresConfirmation = value;
                optionsChanged = true;
            }
        }

        private bool delAllInOpenDocumentsRequiresConfirmation;
        public bool DelAllInOpenDocumentsRequiresConfirmation
        {
            get
            {
                return delAllInOpenDocumentsRequiresConfirmation;
            }
            set
            {
                delAllInOpenDocumentsRequiresConfirmation = value;
                optionsChanged = true;
            }
        }

        private bool delAllInFolderRequiresConfirmation;
        public bool DelAllInFolderRequiresConfirmation
        {
            get
            {
                return delAllInFolderRequiresConfirmation;
            }
            set
            {
                delAllInFolderRequiresConfirmation = value;
                optionsChanged = true;
            }
        }

        private bool delAllInProjectRequiresConfirmation;
        public bool DelAllInProjectRequiresConfirmation
        {
            get
            {
                return delAllInProjectRequiresConfirmation;
            }
            set
            {
                delAllInProjectRequiresConfirmation = value;
                optionsChanged = true;
            }
        }

        private bool delAllInSolutionRequiresConfirmation;
        public bool DelAllInSolutionRequiresConfirmation
        {
            get
            {
                return delAllInSolutionRequiresConfirmation;
            }
            set
            {
                delAllInSolutionRequiresConfirmation = value;
                optionsChanged = true;
            }
        }

        public event EventHandler OptionsChanged;

        protected override void OnApply(PageApplyEventArgs e)
        {
            if(optionsChanged)
                OptionsChanged?.Invoke(this, EventArgs.Empty);

            base.OnApply(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            optionsChanged = false;
            base.OnClosed(e);
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            control.Initialize();
            optionsChanged = false;
            base.OnActivate(e);
        }

        protected override IWin32Window Window => control;

        public string Serialize()
        {
            string Convert(bool value) => value ? "1" : "0";

            return Convert(DelAllInDocumentRequiresConfirmation) +
                   Convert(DelAllInOpenDocumentsRequiresConfirmation) +
                   Convert(DelAllInFolderRequiresConfirmation) +
                   Convert(DelAllInProjectRequiresConfirmation) +
                   Convert(DelAllInSolutionRequiresConfirmation);
        }

        public void Deserialize(string serialized)
        {
            bool Convert(char value) => value == '1';

            DelAllInDocumentRequiresConfirmation = Convert(serialized[0]);
            DelAllInOpenDocumentsRequiresConfirmation = Convert(serialized[1]);
            DelAllInFolderRequiresConfirmation = Convert(serialized[2]);
            DelAllInProjectRequiresConfirmation = Convert(serialized[3]);
            DelAllInSolutionRequiresConfirmation = Convert(serialized[4]);
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
