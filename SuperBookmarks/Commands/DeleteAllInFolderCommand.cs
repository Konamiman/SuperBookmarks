using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInFolderCommand : CommandBase
    {
        protected override int CommandId => 15;

        protected override bool RequiresOpenDocumentsOfAnyKind => true;

        protected override void CommandCallback(OleMenuCommand command)
        {
            BookmarksManager.DeleteAllBookmarksIn(
                Package.Options.DeleteAllInFolderIncludesSubfolders ?
                BookmarkActionTarget.FolderAndSubfolders :
                BookmarkActionTarget.Folder);
        }
    }
}
