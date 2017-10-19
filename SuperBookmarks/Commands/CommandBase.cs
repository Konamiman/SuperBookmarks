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

        protected virtual bool RequiresActiveTextDocument => false;

        protected virtual bool RequiresOpenDocumentsOfAnyKind => false;

        protected virtual bool RequiresOpenTextDocuments => false;

        protected virtual bool RequiresOpenSolution => true;

        protected virtual bool RequiresActiveDocumentToBeInProject => false;

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
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            var command = (OleMenuCommand) sender;

            if (RequiresOpenSolution && !Package.SolutionIsCurrentlyOpen)
            {
                command.Enabled = false;
                return;
            }

            if (RequiresActiveTextDocument && !Package.ActiveDocumentIsText)
            {
                command.Enabled = false;
                return;
            }

            if (RequiresOpenDocumentsOfAnyKind && !Package.ThereAreOpenDocuments)
            {
                command.Enabled = false;
                return;
            }

            if (RequiresOpenTextDocuments && !Package.ThereAreOpenTextDocuments)
            {
                command.Enabled = false;
                return;
            }

            if (RequiresActiveDocumentToBeInProject && !Package.ActiveDocumentIsInProject)
            {
                command.Enabled = false;
                return;
            }

            command.Enabled = true;
            QueryStatusCallback(command);
        }

        protected virtual void QueryStatusCallback(OleMenuCommand command)
        {
        }

        protected abstract void CommandCallback(OleMenuCommand command);
    }
}
