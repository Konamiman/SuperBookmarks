namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInDocCommand : DeleteAllCommandBase
    {
        protected override int CommandId => 13;

        protected override bool RequiresActiveTextDocument => true;

        protected override BookmarkActionTarget Target => BookmarkActionTarget.Document;

        protected override string TargetDisplayName => "the current document";
    }
}
