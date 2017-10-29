using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks.Commands
{
    class SaveToDatCommand : CommandBase
    {
        protected override int CommandId => 20;

        protected override void CommandCallback(OleMenuCommand command)
        {
            const string requiresSettingMessage =
@"Bookmarks are currently saved to the .suo file, which can be written only when the solution closes. Manual save of bookmarks is possible only when these are stored in the .SuperBookmarks.dat file.

Do you want me to open the options page so that you can change where bookmarks are saved?";

            if (!Package.StorageOptions.SaveBookmarksToOwnFile)
            {
                if (Helpers.ShowYesNoQuestionMessage(requiresSettingMessage))
                    this.Package.ShowOptionPage(typeof(StorageOptionsPage));

                return;
            }

            Package.SaveBookmarksToDatFile();
        }
    }
}
