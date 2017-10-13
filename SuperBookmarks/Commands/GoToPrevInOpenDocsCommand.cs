using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToPrevInOpenDocsCommand : CommandBase
    {
        protected override int CommandId => 4;

        protected override bool RequiresOpenDocuments => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToPrevInOpenFiles();
        }
    }
}
