using System;
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Linq;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Editor;

namespace Konamiman.SuperBookmarks
{
    static class Helpers
    {
        private static IServiceProvider serviceProvider = null;

        private static IServiceProvider ServiceProvider =>
            serviceProvider ?? (serviceProvider = SuperBookmarksPackage.Instance);

        //https://msdn.microsoft.com/en-us/library/dd884850.aspx (AddAdornmentHandler)
        public static ITextView GetTextViewForActiveDocument()
        {
            IVsTextManager txtMgr = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
            IVsTextView vTextView = null;
            int mustHaveFocus = 1;
            var x = txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);
            IVsUserData userData = vTextView as IVsUserData;
            if (userData == null)
                return null;

            Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out var holder);
            IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;
            return viewHost.TextView;
        }

        public static TrackingTagSpan<BookmarkTag> CreateTagSpan(ITextBuffer buffer, int lineNumber)
        {
            var snapshot = buffer.CurrentSnapshot;

            //This can happen if the file is edited outside Visual Studio
            //while the solution is closed
            if (lineNumber > snapshot.LineCount)
                return null;

            var line = snapshot.GetLineFromLineNumber(lineNumber - 1);
            var span = snapshot.CreateTrackingSpan(new SnapshotSpan(line.Start, 0), SpanTrackingMode.EdgeExclusive);
            var tagger = GetTaggerFor(buffer);
            var trackingSpan = tagger.CreateTagSpan(span, new BookmarkTag());
            return trackingSpan;
        }
        
        public static SimpleTagger<BookmarkTag> GetTaggerFor(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty("tagger", () => new SimpleTagger<BookmarkTag>(buffer));
        }

        public static bool PathIsInGitRepository(string path)
        {
            return RunGitCommand("rev-parse --is-inside-work-tree", path) == "true";
        }

        public static string GetGitRepositoryRoot(string path)
        {
            return RunGitCommand("rev-parse --show-toplevel", path);
        }

        public static bool AddFileToGitignore(string gitignorePath, string fileToAdd, bool createGitignoreFile)
        {
            var relativeFileToAdd = fileToAdd.Substring(SuperBookmarksPackage.Instance.CurrentSolutionPath.Length);
            var lineToAdd = "\r\n# SuperBookmarks data file\r\n" + relativeFileToAdd + "\r\n";
            if (!File.Exists(gitignorePath))
            {
                if (!createGitignoreFile)
                    return false;

                File.WriteAllText(gitignorePath, lineToAdd);
                return true;
            }

            var gitignoreContents = File.ReadAllText(gitignorePath);
            if (Regex.IsMatch(gitignoreContents, $@"^\s*[^#]?{Regex.Escape(relativeFileToAdd)}\s*$", RegexOptions.Multiline))
                return false;

            File.AppendAllText(gitignorePath, lineToAdd);
            return true;
        }

        //http://stackoverflow.com/a/6119394/4574
        private static string RunGitCommand(string command, string workingDirectory = null)
        {
            var gitInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = $@"/C ""git {command}""",
                UseShellExecute = false
            };

            var gitProcess = new Process();
            gitInfo.WorkingDirectory = workingDirectory;

            gitProcess.StartInfo = gitInfo;
            gitProcess.Start();

            var error = gitProcess.StandardError.ReadToEnd();  // pick up STDERR
            var output = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT

            gitProcess.WaitForExit();
            gitProcess.Close();

            return output == "" ? null : output.Trim('\r', '\n', ' ');
        }

        public static void ShowInfoMessage(string message)
        {
            VsShellUtilities.ShowMessageBox(
                SuperBookmarksPackage.Instance,
                message,
                "SuperBookmarks",
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static void ShowWarningMessage(string message)
        {
            VsShellUtilities.ShowMessageBox(
                SuperBookmarksPackage.Instance,
                message,
                "SuperBookmarks",
                OLEMSGICON.OLEMSGICON_WARNING,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static bool ShowYesNoQuestionMessage(string message)
        {
            const int YesButton = 6;

            return VsShellUtilities.ShowMessageBox(
                SuperBookmarksPackage.Instance,
                message,
                "SuperBookmarks",
                OLEMSGICON.OLEMSGICON_QUERY,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST) == YesButton;
        }

        public static void ShowErrorMessage(string message)
        {
            VsShellUtilities.ShowMessageBox(
                SuperBookmarksPackage.Instance,
                "Something went wrong. The ugly details:\r\n\r\n" + message,
                "SuperBookmarks",
                OLEMSGICON.OLEMSGICON_CRITICAL,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static bool IsTextDocument(ITextBuffer buffer)
        {
            return buffer.ContentType.IsOfType("text");
        }

        public static ITextBuffer GetRootTextBuffer(ITextBuffer buffer)
        {
            if (buffer.ContentType.TypeName == "HTMLXProjection")
            {
                var projectionBuffer = buffer as IProjectionBuffer;
                return projectionBuffer == null
                    ? buffer
                    : projectionBuffer.SourceBuffers.FirstOrDefault(b => Helpers.IsTextDocument(b));
            }
            else
            {
                return Helpers.IsTextDocument(buffer) ? buffer : null;
            }
        }
    }
}
