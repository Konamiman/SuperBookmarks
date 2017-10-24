using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Konamiman.SuperBookmarks.Commands
{
    internal abstract class CommandBase
    {
        static CommandBase()
        {
            CommandsGuidString = CommandsGuid.ToString();
        }

        public static string CommandsGuidString { get; }

        public const string TheCommandsGuid = "{ce119b9b-9a23-444f-80cb-945bd56d9b6e}";

        public static Guid CommandsGuid { get; } = Guid.Parse(TheCommandsGuid);

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

            command.Visible = true;
            command.Enabled = true;
            QueryStatusCallback(command);
            if (!command.Enabled || !command.Visible)
                return;

            if (
                (RequiresOpenSolution && !Package.SolutionIsCurrentlyOpen) ||
                (RequiresActiveTextDocument && !Package.ActiveDocumentIsText) ||
                (RequiresOpenDocumentsOfAnyKind && !Package.ThereAreOpenDocuments) ||
                (RequiresOpenTextDocuments && !Package.ThereAreOpenTextDocuments) ||
                (RequiresActiveDocumentToBeInProject && !Package.ActiveDocumentIsInProject))
            {
                command.Enabled = false;
            }
        }

        protected virtual void QueryStatusCallback(OleMenuCommand command)
        {
        }

        protected abstract void CommandCallback(OleMenuCommand command);
    }
}
