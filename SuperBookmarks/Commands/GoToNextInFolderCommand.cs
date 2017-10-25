using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToNextInFolderCommand : CommandBase
    {
        protected override int CommandId => 7;

        protected override bool RequiresOpenDocumentsOfAnyKind => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToNextIn(
                Package.Options.NavigateInFolderIncludesSubfolders ?
                BookmarkActionTarget.FolderAndSubfolders :
                BookmarkActionTarget.Folder);
        }
    }
}
