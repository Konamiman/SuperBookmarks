using Microsoft.VisualStudio.Shell;
using System.IO;

namespace Konamiman.SuperBookmarks.Commands
{
    class LoadFromDatCommand : CommandBase
    {
        protected override int CommandId => 21;

        const string requiresSettingMessage =
@"Bookmarks are currently saved to the .suo file, which can be read only when the solution opens. Manual load of bookmarks is possible only when these are stored in the .SuperBookmarks.dat file.

Do you want me to open the options page so that you can change where bookmarks are saved?";

        const string noFileExistsMessage =
"No .SuperBookmarks.dat file exists currently. A new one will be created when the solution is closed, or you can force it to be created by executing the \"Save Bookmarks to .dat file\" command.";

        protected override void CommandCallback(OleMenuCommand command)
        {
            if(!Package.StorageOptions.SaveBookmarksToOwnFile)
            {
                if (Helpers.ShowYesNoQuestionMessage(requiresSettingMessage))
                    this.Package.ShowOptionPage(typeof(StorageOptionsPage));

                return;
            }

            if(!File.Exists(Package.DataFilePath))
            {
                Helpers.ShowInfoMessage(noFileExistsMessage);
                return;
            }

            if (Package.ConfirmationOptions.ReplacingLoadRequiresConfirmation)
            {
                var existingCount = BookmarksManager.GetBookmarksCount(BookmarkActionTarget.Solution);
                if (existingCount != 0)
                {
                    var fileCount = BookmarksManager.GetFilesWithBookmarksCount();
                    var message =
$@"{Helpers.Quantifier(existingCount, @"existing bookmark")} in {Helpers.Quantifier(fileCount, "file")} will be replaced with the bookmarks read from the .SuperBookmarks.dat file.
Are you sure?";
                    if (!Helpers.ShowYesNoQuestionMessage(message))
                        return;
                }
            }

            Package.LoadBookmarksFromDatFile();
        }
    }
}
