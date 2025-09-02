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

        private int consoleLeft;
        private int consoleTop;

        // Constructor
        public SyncMonitor(string src, string dest)
        {
            source = src;
            destination = dest;

            consoleLeft = 0;
        }

        // Activate daemon mode
        public void activateDaemon()
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[ACTIVATE DAEMON]");
            Console.ForegroundColor = ConsoleColor.Gray;

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
            if (isFile(e.FullPath))
            {
                printOverwrite(destName);
                copyFile(e.FullPath, destName, true);
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

        // Reset cursor position
        private void resetCursor()
        {
            consoleTop = Console.GetCursorPosition().Top;
            Console.SetCursorPosition(consoleLeft, consoleTop);
        }

        // Print copy
        private void printCopy(string file)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[CP] ");
            printItemName(file);
        }

        // Print make directory
        private void printMakeDir(string dir)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[MK DIR] ");
            printItemName(dir);
        }

        // Print overwrite
        private void printOverwrite(string file)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[OVERWRITE] ");
            printItemName(file);
        }

        // Print rename
        private void printRename(string oldName, string newName)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[RENAME] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{oldName} -> {newName}");
        }

        // Print delete
        private void printDelete(string name)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[DEL] ");
            printItemName(name);
        }

        // Print file name
        private void printItemName(string item)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{item}");
        }

        // Error
        private void error(Exception e)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{e.Message}");
        }
    }
}
