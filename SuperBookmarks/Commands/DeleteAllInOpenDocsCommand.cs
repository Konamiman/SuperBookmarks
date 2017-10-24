using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInOpenDocsCommand : CommandBase
    {
        protected override int CommandId => 14;

        protected override bool RequiresOpenTextDocuments => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.DeleteAllBookmarksIn(BookmarkActionTarget.OpenDocuments);
        }
    }
}
