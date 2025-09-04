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
            if (isFile(e.FullPath) && !renamedFile)
            {
                printOverwrite(destName);
                copyFile(e.FullPath, destName, true);
            }
            else if (isFile(e.FullPath))
            {
                renamedFile = false;
            }
        }

        // Created callback
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            if (isFile(e.FullPath))
            {
                printCopy(destName);
                copyFile(e.FullPath, destName, false);
            }
            else if (isDirectory(e.FullPath))
            {
                printMakeDir(destName);
                createDirectory(destName);
            }
        }

        // Renamed callback
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string oldDestName = destination + e.OldFullPath.Substring(source.Length);
            string newDestName = destination + e.FullPath.Substring(source.Length);
            if (isFile(e.FullPath))
            {
                printRename(oldDestName, newDestName);
                renameFile(oldDestName, newDestName);
                renamedFile = true;
            }
            else if (isDirectory(e.FullPath))
            {
                printRename(oldDestName, newDestName);
                renameDirectory(oldDestName, newDestName);
            }
        }

        // Deleted callback
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            if (isFile(destName))
            {
                printDelete(destName);
                deleteFile(destName);
            }
            else if (isDirectory(destName))
            {
                printDelete(destName);
                deleteDirectory(destName);
            }
        }

        // Error callback
        private void OnError(object sender, ErrorEventArgs e)
        {
            throw new Exception(e.GetException().Message);
        }

        // Check if name is file
        private bool isFile(string name)
        {
            try
            {
                if (File.Exists(name))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                error(e);
            }
            return false;
        }

        // Check if name is directory
        private bool isDirectory(string name) {
            try
            {
                if (Directory.Exists(name))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                error(e);
            }
            return false;
        }

        // Make directory
        private void createDirectory(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception e)
            {
                error(e);
            }
        }

        // Rename directory
        private void renameDirectory(string oldDir, string newDir)
        {
            try
            {
                Directory.Move(oldDir, newDir);
            }
            catch (Exception e)
            {
                error(e);
            }
        }

        // Delete directory
        private void deleteDirectory(string dir)
        {
            DirectoryInfo directory = new DirectoryInfo(dir);
            DirectoryInfo[] subdirectories = directory.GetDirectories("*.*", SearchOption.AllDirectories);
            FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);

            try
            {
                foreach (FileInfo file in files)
                {
                    deleteFile(file.FullName);
                }

                foreach (DirectoryInfo subdirectory in subdirectories)
                {
                    Directory.Delete(subdirectory.FullName);
                }

                Directory.Delete(dir);
            }
            catch (Exception e)
            {
                error(e);
            }
        }

        // Copy file
        private void copyFile(string srcFile, string destFile, bool overwrite)
        {
            try
            {
                File.Copy(srcFile, destFile, overwrite);
            }
            catch (Exception e)
            {
                error(e);
            }
        }

        // Rename file
        private void renameFile(string oldFile, string newFile)
        {
            try
            {
                File.Move(oldFile, newFile);
            }
            catch (Exception e)
            {
                error(e);
            }
        }

        // Delete file
        private void deleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
                error(e);
            }
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
        
        // Error
        private void error(Exception e)
        {
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Yellow, $"[ERROR] ",
                                            ConsoleColor.Gray, $"{e.Message}");
        }
    }
}
