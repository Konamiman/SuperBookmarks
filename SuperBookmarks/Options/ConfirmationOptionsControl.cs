using System.Windows.Forms;

namespace Konamiman.SuperBookmarks.Options
{
    public partial class ConfirmationOptionsControl : UserControl
    {
        public ConfirmationOptionsControl()
        {
            InitializeComponent();
        }

        internal ConfirmationsPage Options { get; set; }

        public void Initialize()
        {
            chkDocument.Checked = Options.DelAllInDocumentRequiresConfirmation;
            chkOpenFiles.Checked = Options.DelAllInOpenDocumentsRequiresConfirmation;
            chkFolder.Checked = Options.DelAllInFolderRequiresConfirmation;
            chkProject.Checked = Options.DelAllInProjectRequiresConfirmation;
            chkSolution.Checked = Options.DelAllInSolutionRequiresConfirmation;

            chkDocument.CheckedChanged += (sender, args) =>
                Options.DelAllInDocumentRequiresConfirmation = chkDocument.Checked;

            chkOpenFiles.CheckedChanged += (sender, args) =>
                Options.DelAllInOpenDocumentsRequiresConfirmation = chkOpenFiles.Checked;

            chkFolder.CheckedChanged += (sender, args) =>
                Options.DelAllInFolderRequiresConfirmation = chkFolder.Checked;

            chkProject.CheckedChanged += (sender, args) =>
                Options.DelAllInProjectRequiresConfirmation = chkProject.Checked;

            chkSolution.CheckedChanged += (sender, args) =>
                Options.DelAllInSolutionRequiresConfirmation = chkSolution.Checked;
        }
    }
}
