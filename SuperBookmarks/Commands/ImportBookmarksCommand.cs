using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudioTools;
using System.IO;

namespace Konamiman.SuperBookmarks.Commands
{
    class ImportBookmarksCommand : CommandBase
    {
        protected override int CommandId => 19;
        
        protected override void CommandCallback(OleMenuCommand command)
        {
            var fileName = FileDialogs.BrowseForFileOpen(IntPtr.Zero, ".dat files|*.dat|All files|*.*", Package.GetLastUsedExportImportFolder(), "Import Bookmarks");
            if (fileName == null)
                return;

            SerializableBookmarksInfo info;
            try
            {
                using (var stream = File.OpenRead(fileName))
                    info = SerializableBookmarksInfo.DeserializeFrom(stream);
            }
            catch
            {
                Helpers.ShowErrorMessage("Sorry, I couldn't parse the file. Perhaps it is malformed?", showHeader: false);
                return;
            }

            if (!Package.Options.MergeWhenImporting)
            {
                if (Package.ConfirmationOptions.ReplacingImportRequiresConfirmation)
                {
                    var existingCount = BookmarksManager.GetBookmarksCount(BookmarkActionTarget.Solution);
                    if (existingCount != 0)
                    {
                        var newCount = info.TotalBookmarksCount;
                        var message =
$@"{existingCount} existing bookmark{(existingCount == 1 ? "" : "s")} will be replaced with {newCount} bookmark{(newCount == 1 ? "" : "s")} from the file.
Are you sure?";
                        if (!Helpers.ShowYesNoQuestionMessage(message))
                            return;
                    }
                }

                this.BookmarksManager.DeleteAllBookmarksIn(BookmarkActionTarget.Solution);
            }

            this.BookmarksManager.RecreateBookmarksFromSerializedInfo(info);

            Package.SetLastUsedExportImportFolder(Path.GetDirectoryName(fileName));
        }
    }
}
