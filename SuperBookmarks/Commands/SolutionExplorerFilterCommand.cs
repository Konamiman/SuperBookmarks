using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class SolutionExplorerFilterCommand : CommandBase
    {
        public const int TheCommandId = 12;

        protected override int CommandId => TheCommandId;

        protected override void QueryStatusCallback(OleMenuCommand command)
        {
            if (!Package.SolutionIsCurrentlyOpen)
                command.Visible = false;
        }

        protected override void CommandCallback(OleMenuCommand command)
        {
            //Nothing to do here, everything is handled by SolutionExplorerFilter
            //(this class exists solely to control the button visibility)
        }
    }
}
