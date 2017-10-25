namespace Konamiman.SuperBookmarks.Commands
{
    class DeleteAllInSolutionCommand : DeleteAllCommandBase
    {
        protected override int CommandId => 17;

        protected override bool RequiresOpenSolution => true;

        protected override BookmarkActionTarget Target => BookmarkActionTarget.Solution;

        protected override string TargetDisplayName => "the solution";
    }
}
