namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInProjectCommand : DeleteAllCommandBase
    {
        protected override int CommandId => 16;

        protected override bool RequiresActiveDocumentToBeInProject => true;

        protected override BookmarkActionTarget Target => BookmarkActionTarget.Project;

        protected override string TargetDisplayName => "the current project";
    }
}
