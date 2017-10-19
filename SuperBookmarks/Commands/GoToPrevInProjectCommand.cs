using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToPrevInProjectCommand : CommandBase
    {
        protected override int CommandId => 8;

        protected override bool RequiresActiveDocumentToBeInProject => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToPrevInProject();
        }
    }
}
