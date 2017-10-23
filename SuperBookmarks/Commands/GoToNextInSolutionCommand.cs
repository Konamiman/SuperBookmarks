using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToNextInSolutionCommand : CommandBase
    {
        protected override int CommandId => 11;

        protected override bool RequiresOpenSolution => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToNextIn(BookmarkActionTarget.Solution);
        }
    }
}
