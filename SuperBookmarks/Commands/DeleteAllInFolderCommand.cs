namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInFolderCommand : DeleteAllCommandBase
    {
        protected override int CommandId => 15;

        protected override bool RequiresOpenDocumentsOfAnyKind => true;

        protected override BookmarkActionTarget Target =>
            Package.Options.DeleteAllInFolderIncludesSubfolders ?
                BookmarkActionTarget.FolderAndSubfolders :
                BookmarkActionTarget.Folder;

        protected override string TargetDisplayName =>
            Package.Options.DeleteAllInFolderIncludesSubfolders ? 
            "the current folder (including subfolders)" :
            "the current folder";
    }
}
