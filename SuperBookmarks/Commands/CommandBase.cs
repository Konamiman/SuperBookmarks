using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Konamiman.SuperBookmarks.Commands
{
    internal abstract class CommandBase
    {
        protected Guid CommandsGuid = new Guid("ce119b9b-9a23-444f-80cb-945bd56d9b6e");

        protected abstract int CommandId { get; }

        protected BookmarksManager BookmarksManager { get; }

        protected SuperBookmarksPackage Package { get; }

        public CommandBase()
        {
            this.Package = SuperBookmarksPackage.Instance;
            this.BookmarksManager = this.Package.BookmarksManager;

            var commandService = 
                ((IServiceProvider)SuperBookmarksPackage.Instance)
                .GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null)
                return;

            var menuCommandID = new CommandID(CommandsGuid, CommandId);
            var menuItem = new OleMenuCommand((sender, eventArgs) => CommandCallback((OleMenuCommand)sender), menuCommandID);
            commandService.AddCommand(menuItem);
        }

        protected abstract void CommandCallback(OleMenuCommand command);
    }
}
