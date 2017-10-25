using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class GoToPrevInFolderCommand : CommandBase
    {
        protected override int CommandId => 6;

        protected override bool RequiresOpenDocumentsOfAnyKind => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.GoToPrevIn(
                Package.Options.NavigateInFolderIncludesSubfolders ?
                BookmarkActionTarget.FolderAndSubfolders :
                BookmarkActionTarget.Folder);
        }
    }
}
