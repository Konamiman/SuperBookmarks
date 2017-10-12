using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToPrevInDocCommand : CommandBase
    {
        protected override int CommandId => 2;

        protected override bool RequiresOpenTextDocument => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToPrevInCurrentDocument();
        }
    }
}
