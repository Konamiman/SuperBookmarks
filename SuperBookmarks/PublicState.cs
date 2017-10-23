namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        public bool SolutionIsCurrentlyOpen { get; private set; }
        public string CurrentSolutionPath { get; private set; }
        public string CurrentSolutionSuoPath { get; private set; }
        public bool CurrentSolutionIsInGitRepo { get; private set; }
        public bool ThereAreOpenDocuments { get; private set; }
        public bool ThereAreOpenTextDocuments { get; private set; }
        public bool ActiveDocumentIsText { get; private set; } = false;
        public bool ActiveDocumentIsInProject { get; private set; } = false;
    }
}
