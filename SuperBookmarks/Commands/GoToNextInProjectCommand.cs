using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToNextInProjectCommand : CommandBase
    {
        protected override int CommandId => 9;

        protected override bool RequiresActiveDocumentToBeInProject => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToNextIn(BookmarkActionTarget.Project);
        }
    }
}
