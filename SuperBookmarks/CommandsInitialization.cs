using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konamiman.SuperBookmarks.Commands;
using Microsoft.VisualStudio.Shell;

namespace Konamiman.SuperBookmarks
{
    public partial class SuperBookmarksPackage
    {
        private CommandBase[] Commands;

        void InitializeMenu()
        {
            //Both menus are declared as "default invisible" in the .vsct file,
            //therefore only the one we register now will show.

            const int topLevelMenuId = 1;
            const int submenuId = 2;

            var menuToShow = Options.ShowCommandsInTopLevelMenu
                ? topLevelMenuId
                : submenuId;

            var commandService = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var menusGuid = new Guid("391D1519-100C-4656-AC82-F3DA9998E55B");
            var menuCommandID = new CommandID(menusGuid, menuToShow);
            var menuItem = new OleMenuCommand(null, menuCommandID);
            menuItem.BeforeQueryStatus += (sender, args) => menuItem.Visible = true;
            commandService.AddCommand(menuItem);
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

                new OpenOptionsCommand()
            };
        }
    }
}
