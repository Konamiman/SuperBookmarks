namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInOpenDocsCommand : DeleteAllCommandBase
    {
        protected override int CommandId => 14;

        protected override bool RequiresOpenTextDocuments => true;

        protected override BookmarkActionTarget Target => BookmarkActionTarget.OpenDocuments;

        protected override string TargetDisplayName => "all open documents";
    }
}
