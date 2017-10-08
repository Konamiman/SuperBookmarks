using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class OpenOptionsCommand : CommandBase
    {
        protected override int CommandId => 100;

        protected override void CommandCallback(OleMenuCommand command)
        {
            this.Package.ShowOptionPage(typeof(OptionsPage));
        }
    }
}
