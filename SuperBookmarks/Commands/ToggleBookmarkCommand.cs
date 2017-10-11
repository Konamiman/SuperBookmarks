using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class ToggleBookmarkCommand : CommandBase
    {
        protected override int CommandId => 1;

        protected override bool RequiresOpenTextDocument => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.SetOrRemoveBookmarkInCurrentDocument();
        }
    }
}
