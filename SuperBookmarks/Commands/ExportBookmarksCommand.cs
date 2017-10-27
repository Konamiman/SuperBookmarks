using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudioTools;
using System.IO;

namespace Konamiman.SuperBookmarks.Commands
{
    class ExportBookmarksCommand : CommandBase
    {
        protected override int CommandId => 18;

        protected override void CommandCallback(OleMenuCommand command)
        {
            var count = BookmarksManager.GetBookmarksCount(BookmarkActionTarget.Solution);
            if (count == 0)
            {
                Helpers.ShowInfoMessage("There are no bookmarks to export");
                return;
            }

            var fileName = FileDialogs.BrowseForFileSave(IntPtr.Zero, ".dat files|*.dat|All files|*.*", Package.GetLastUsedExportImportFolder(), "Export Bookmarks");
            if (fileName != null)
            {
                var info = this.BookmarksManager.GetSerializableInfo();
                using (var stream = File.Create(fileName))
                    info.SerializeTo(stream, prettyPrint: true);

                Helpers.WriteToStatusBar($"{Helpers.Quantifier(info.TotalBookmarksCount, "bookmark")} from {Helpers.Quantifier(info.TotalFilesCount, "file")} have been exported to {Path.GetFileName(fileName)}");

                Package.SetLastUsedExportImportFolder(Path.GetDirectoryName(fileName));
            }
        }
    }
}
