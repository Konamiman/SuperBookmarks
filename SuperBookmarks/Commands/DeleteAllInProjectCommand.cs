using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInProjectCommand : CommandBase
    {
        protected override int CommandId => 16;

        protected override bool RequiresActiveDocumentToBeInProject => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.DeleteAllBookmarksIn(BookmarkActionTarget.Project);
        }
    }
}
