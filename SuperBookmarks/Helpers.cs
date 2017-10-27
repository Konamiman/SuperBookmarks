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
using System.Collections.Generic;
using System.Globalization;

namespace Konamiman.SuperBookmarks
{
    static class Helpers
    {
        private static IServiceProvider serviceProvider = null;
        private static IVsStatusbar statusBar = null;

        private static IServiceProvider ServiceProvider =>
            serviceProvider ?? (serviceProvider = SuperBookmarksPackage.Instance);

        private static IVsStatusbar StatusBar =>
            statusBar ?? (statusBar = (IVsStatusbar)ServiceProvider.GetService(typeof(SVsStatusbar)));


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

        public static void ShowInfoMessage(string message, bool showTitle = true)
        {
            VsShellUtilities.ShowMessageBox(
                SuperBookmarksPackage.Instance,
                message,
                showTitle ? "SuperBookmarks" : null,
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

        public static bool ShowYesNoQuestionMessage(string message, bool showTitle = false)
        {
            const int YesButton = 6;

            return VsShellUtilities.ShowMessageBox(
                SuperBookmarksPackage.Instance,
                message,
                showTitle ? "SuperBookmarks" : null,
                OLEMSGICON.OLEMSGICON_QUERY,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST) == YesButton;
        }

        public static void ShowErrorMessage(string message, bool showTitle = false, bool showHeader = true)
        {
            VsShellUtilities.ShowMessageBox(
                SuperBookmarksPackage.Instance,
                showHeader ?
                    "Something went wrong. The ugly details:\r\n\r\n" + message :
                    message,
                showTitle ? "SuperBookmarks" : null,
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

        private static Dictionary<string, string> properlyCasedPaths = new Dictionary<string, string>();

        public static void ClearProperlyCasedPathsCache()
        {
            properlyCasedPaths.Clear();
        }

        public static string GetProperlyCasedPath(string path)
        {
            if(properlyCasedPaths.TryGetValue(path, out var properlyCasedPath))
                return properlyCasedPath;

            try
            {
                properlyCasedPath = GetProperlyCasedPathCore(path);
                if (properlyCasedPath == null)
                {
                    Helpers.ShowErrorMessage($"I couldn't get the properly cased version of '{path}' - bookmark navigation might not work properly");
                    properlyCasedPath = path;
                }
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage($"Error when trying to get the properly cased version of path '{path}':\r\n\r\n{ex.Message}");
                properlyCasedPath = path;
            }
            
            properlyCasedPaths.Add(path, properlyCasedPath);
            return properlyCasedPath;
        }

        //https://stackoverflow.com/a/29578292/4574
        private static string GetProperlyCasedPathCore(string path)
        {
            // DirectoryInfo accepts either a file path or a directory path, and most of its properties work for either.
            // However, its Exists property only works for a directory path.
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!File.Exists(path) && !directory.Exists)
                return null;

            List<string> parts = new List<string>();

            DirectoryInfo parentDirectory = directory.Parent;
            while (parentDirectory != null)
            {
                FileSystemInfo entry = parentDirectory.EnumerateFileSystemInfos(directory.Name).First();
                parts.Add(entry.Name);

                directory = parentDirectory;
                parentDirectory = directory.Parent;
            }

            // Handle the root part (i.e., drive letter or UNC \\server\share).
            string root = directory.FullName;
            if (root.Contains(':'))
            {
                root = root.ToUpper();
            }
            else
            {
                string[] rootParts = root.Split('\\');
                root = string.Join("\\", rootParts.Select(part => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(part)));
            }

            parts.Add(root);
            parts.Reverse();
            return Path.Combine(parts.ToArray());
        }

        public static string Quantifier(int count, string singularTerm)
            => $"{count} {singularTerm}{(count == 1 ? "" : "s")}";

        public static void WriteToStatusBar(string message)
        {
            StatusBar.IsFrozen(out int frozen);
            if (frozen != 0)
                StatusBar.FreezeOutput(0);

            StatusBar.SetText(message);

            StatusBar.FreezeOutput(1);
        }
    }
}
