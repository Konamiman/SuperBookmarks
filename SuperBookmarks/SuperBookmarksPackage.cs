using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;

namespace Konamiman.SuperBookmarks
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(SuperBookmarksPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideOptionPage(typeof(OptionsPage), "SuperBookmarks", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(StorageOptionsPage), "SuperBookmarks", "Storage", 0, 0, true)]
    public sealed partial class SuperBookmarksPackage : Package,
        IVsPersistSolutionOpts,
        IVsSolutionEvents,
        IVsTrackProjectDocumentsEvents2
    {
        /// <summary>
        /// SuperBookmarksPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "b634341b-bb5b-463e-9886-f11c4391941e";

        private const string SettingsStoreName = "Konamiman.SuperBookmarks";

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperBookmarksPackage"/> class.
        /// </summary>
        public SuperBookmarksPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            Instance = this;
            BookmarksManager = new BookmarksManager(this);
        }

        #region Package Members

        private IVsSolution solutionService;

        public bool SolutionIsCurrentlyOpen { get; private set; }
        public string CurrentSolutionPath { get; private set; }
        public string CurrentSolutionSuoPath { get; private set; }

        public bool CurrentSolutionIsInGitRepo { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            solutionService = (IVsSolution) GetService(typeof(SVsSolution));
            solutionService.AdviseSolutionEvents(this, out var cookie);

            var tpdService = (IVsTrackProjectDocuments2)GetService(typeof(SVsTrackProjectDocuments));
            tpdService.AdviseTrackProjectDocumentsEvents(this, out var pdwCookie);

            InitializeOptionsStorage();

            BookmarksManager.InitializeAfterPackageInitialization();

            InitializeMenu();
            InitializeCommands();

            var dte = GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            dte.Events.WindowEvents.WindowActivated += WindowEventsOnWindowActivated;
        }

        public bool ActiveDocumentIsText { get; private set; } = false;

        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        private void WindowEventsOnWindowActivated(Window gotFocus, Window lostFocus)
        {
            //TODO: Find a better way to do this. Currently it's returning false positives when a designer window is open.

            if (gotFocus.Kind != "Document")
                return; //It's not a document (e.g. a tool window)

            ActiveDocumentIsText = gotFocus.Document.Object("TextDocument") != null;
        }

        public static SuperBookmarksPackage Instance { get; private set; }

        internal BookmarksManager BookmarksManager { get; }

        #endregion
    }
}
