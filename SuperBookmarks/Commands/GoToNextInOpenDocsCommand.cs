using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToNextInOpenDocsCommand : CommandBase
    {
        protected override int CommandId => 5;

        protected override bool RequiresOpenDocuments => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToNextInOpenFiles();
        }
    }
}
