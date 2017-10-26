using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools;

namespace Konamiman.SuperBookmarks.Commands
{
    class ToggleBookmarkCommand : CommandBase
    {
        protected override int CommandId => 1;

        protected override bool RequiresActiveTextDocument => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.SetOrRemoveBookmarkInCurrentDocument();
        }
    }
}
