using System;
using System.ComponentModel.Design;
using Konamiman.SuperBookmarks.Commands;
using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        private CommandBase[] Commands;

        const int menuId = 1;
        private readonly static Guid menuGuid = new Guid("391D1519-100C-4656-AC82-F3DA9998E55B");
        private CommandID menuCommandId = new CommandID(menuGuid, menuId);

        void InitializeMenu()
        {
            var commandService = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var menuItem = new OleMenuCommand(null, menuCommandId);
            menuItem.Visible = false;
            commandService.AddCommand(menuItem);
            UpdateMenuVisibilityAndText();
        }

        private void UpdateMenuVisibilityAndText() =>
            Helpers.SafeInvoke(_UpdateMenuVisibilityAndText);

        private void _UpdateMenuVisibilityAndText()
        {
            var commandService = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var menu = (OleMenuCommand)commandService.FindCommand(menuCommandId);
            menu.Visible = Options.ShowMenuOption != ShowMenuOption.DontShow;
            menu.Text = Options.ShowMenuOption == ShowMenuOption.WithTitleBookmarks ?
                "Bookmarks" : "SuperBookmarks";
        }

        void InitializeCommands()
        {
            Commands = new CommandBase[]
            {
                new ToggleBookmarkCommand(),
                new GoToPrevInDocCommand(),
                new GoToNextInDocCommand(),
                new GoToPrevInOpenDocsCommand(),
                new GoToNextInOpenDocsCommand(),
                new GoToPrevInFolderCommand(),
                new GoToNextInFolderCommand(),
                new GoToPrevInProjectCommand(),
                new GoToNextInProjectCommand(),
                new GoToPrevInSolutionCommand(),
                new GoToNextInSolutionCommand(),

                new SolutionExplorerFilterCommand(),

                new DeleteAllInDocCommand(), 
                new DeleteAllInOpenDocsCommand(), 
                new DeleteAllInFolderCommand(), 
                new DeleteAllInProjectCommand(), 
                new DeleteAllInSolutionCommand(), 

                new ExportBookmarksCommand(),
                new ImportBookmarksCommand(),
                new SaveToDatCommand(),
                new LoadFromDatCommand(),

                new OpenOptionsCommand()
            };
        }
    }
}
