using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToNextInOpenDocsCommand : CommandBase
    {
        protected override int CommandId => 5;

        protected override bool RequiresOpenTextDocuments => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToNextIn(BookmarkActionTarget.OpenDocuments);
        }
    }
}
