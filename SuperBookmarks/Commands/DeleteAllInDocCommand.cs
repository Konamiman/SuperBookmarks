using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInDocCommand : CommandBase
    {
        protected override int CommandId => 13;

        protected override bool RequiresActiveTextDocument => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.DeleteAllBookmarksIn(BookmarkActionTarget.Document);
        }
    }
}
