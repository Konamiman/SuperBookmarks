using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    abstract class DeleteAllCommandBase : CommandBase
    {
        protected abstract BookmarkActionTarget Target { get; }

        protected abstract string TargetDisplayName { get; }

        protected override void CommandCallback(OleMenuCommand command)
        {
            var shouldAskConfirmation = Package.ConfirmationOptions.ShouldConfirmForDeleteAllIn(Target);
            if (!shouldAskConfirmation)
            {
                BookmarksManager.DeleteAllBookmarksIn(Target);
                return;
            }

            string message;
            var count = BookmarksManager.GetBookmarksCount(Target);
            if (count == 0)
            {
                message = $"There are no bookmarks in {TargetDisplayName}";
                Helpers.ShowInfoMessage(message);
                return;
            }

            message = 
$@"There {(count == 1 ? "is 1 bookmark" : $"are {count} bookmarks")} in {TargetDisplayName}.
Do you want to delete {(count == 1 ? "it" : "all of them")}?";
            if (Helpers.ShowYesNoQuestionMessage(message))
                BookmarksManager.DeleteAllBookmarksIn(Target);
        }
    }
}
