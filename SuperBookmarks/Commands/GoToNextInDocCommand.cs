using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToNextInDocCommand : CommandBase
    {
        protected override int CommandId => 3;

        protected override bool RequiresActiveTextDocument => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToNextIn(BookmarkActionTarget.Document);
        }
    }
}
