// SyncMonitor.cs
// Contains the SyncMonitor class

using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FileUtils
{
    public class SyncMonitor
    {
        // Members
        string source = "";
        FileSystemWatcher srcWatcher;

        string destination = "";

        bool renamedFile = false;

        // Constructor
        public SyncMonitor(string src, string dest)
        {
            source = src;
            destination = dest;
        }

        // Activate daemon mode
        public void activateDaemon()
        {
            ConsoleLogger.WriteLine(ConsoleColor.Cyan, "[ACTIVATE DAEMON]");
            setUpSrcWatcher();
        }

        // Set up source watcher
        private void setUpSrcWatcher()
        {
            srcWatcher = new FileSystemWatcher(source);
            srcWatcher.NotifyFilter = NotifyFilters.Attributes
                                    | NotifyFilters.CreationTime
                                    | NotifyFilters.DirectoryName
                                    | NotifyFilters.FileName
                                    | NotifyFilters.LastAccess
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.Security
                                    | NotifyFilters.Size;
            srcWatcher.Changed += OnChanged;
            srcWatcher.Created += OnCreated;
            srcWatcher.Renamed += OnRenamed;
            srcWatcher.Deleted += OnDeleted;
            srcWatcher.Error += OnError;
            srcWatcher.IncludeSubdirectories = true;
            srcWatcher.EnableRaisingEvents = true;
        }

        // Changed callback
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            if (FileOps.IsFile(e.FullPath) && !renamedFile)
            {
                printOverwrite(destName);
                FileOps.CopyFile(e.FullPath, destName, true);
            }
            else if (FileOps.IsFile(e.FullPath))
            {
                renamedFile = false;
            }
        }

        // Created callback
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            if (FileOps.IsFile(e.FullPath))
            {
                printCopy(destName);
                FileOps.CopyFile(e.FullPath, destName, false);
            }
            else if (DirectoryOps.IsDirectory(e.FullPath))
            {
                printMakeDir(destName);
                DirectoryOps.CreateDirectory(destName);
            }
        }

        // Renamed callback
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string oldDestName = destination + e.OldFullPath.Substring(source.Length);
            string newDestName = destination + e.FullPath.Substring(source.Length);
            if (FileOps.IsFile(e.FullPath))
            {
                printRename(oldDestName, newDestName);
                FileOps.RenameFile(oldDestName, newDestName);
                renamedFile = true;
            }
            else if (DirectoryOps.IsDirectory(e.FullPath))
            {
                printRename(oldDestName, newDestName);
                DirectoryOps.RenameDirectory(oldDestName, newDestName);
            }
        }

        // Deleted callback
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            if (FileOps.IsFile(destName))
            {
                printDelete(destName);
                FileOps.DeleteFile(destName);
            }
            else if (DirectoryOps.IsDirectory(destName))
            {
                printDelete(destName);
                DirectoryOps.DeleteDirectoryRecursive(destName);
            }
        }

        // Error callback
        private void OnError(object sender, ErrorEventArgs e)
        {
            throw new Exception(e.GetException().Message);
        }

        // Print copy
        private void printCopy(string file)
        {
            DateTime t = DateTime.Now;
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Green, $"[CP | {t}]",
                                            ConsoleColor.Gray, $"{file}");
        }

        // Print make directory
        private void printMakeDir(string dir)
        {
            DateTime t = DateTime.Now;
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Green, $"[MKDIR | {t}] ",
                                            ConsoleColor.Gray, $"{dir}");
        }

        // Print overwrite
        private void printOverwrite(string file)
        {
            DateTime t = DateTime.Now;
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Yellow, $"[OVRWRT | {t}] ",
                                            ConsoleColor.Gray, $"{file}");
        }

        // Print rename
        private void printRename(string oldName, string newName)
        {
            DateTime t = DateTime.Now;
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Yellow, $"[RENAME | {t}] ",
                                            ConsoleColor.Gray, $"{oldName} -> {newName}");
        }

        // Print delete
        private void printDelete(string name)
        {
            DateTime t = DateTime.Now;
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Yellow, $"[DEL | {t}] ",
                                            ConsoleColor.Gray, $"{name}");
        }
    }
}
