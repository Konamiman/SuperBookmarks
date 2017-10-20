using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToPrevInSolutionCommand : CommandBase
    {
        protected override int CommandId => 10;

        protected override bool RequiresOpenSolution => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToPrevInSolution();
        }
    }
}
