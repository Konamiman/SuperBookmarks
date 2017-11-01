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
$@"{Helpers.Quantifier(existingCount, @"existing bookmark")} will be replaced with {Helpers.Quantifier(newCount, "bookmark")} from the file.
Are you sure?";
                        if (!Helpers.ShowYesNoQuestionMessage(message))
                            return;
                    }
                }

                this.BookmarksManager.DeleteAllBookmarksIn(BookmarkActionTarget.Solution);
            }

            try
            {
                this.BookmarksManager.RecreateBookmarksFromSerializedInfo(info);
                Helpers.WriteToStatusBar($"Import done, solution has now {Helpers.Quantifier(BookmarksManager.GetBookmarksCount(BookmarkActionTarget.Solution), "bookmark")} in {Helpers.Quantifier(BookmarksManager.GetFilesWithBookmarksCount(), "file")}");
            }
            catch
            {
                Helpers.ShowErrorMessage("Sorry, I couldn't get bookmarks information from the file. Perhaps it is malformed?", showHeader: false);
                return;
            }

            Package.SetLastUsedExportImportFolder(Path.GetDirectoryName(fileName));
        }
    }
}
