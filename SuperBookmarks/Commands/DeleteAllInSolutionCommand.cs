using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInSolutionCommand : CommandBase
    {
        protected override int CommandId => 17;

        protected override bool RequiresOpenSolution => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.DeleteAllBookmarksIn(BookmarkActionTarget.Solution);
        }
    }
}
