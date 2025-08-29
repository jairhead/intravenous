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
        static string source = "";
        FileSystemWatcher srcWatcher;

        static string destination = "";

        static private int consoleLeft;
        static private int consoleTop;

        // Constructor
        public SyncMonitor(string src, string dest)
        {
            source = src;
            destination = dest;

            consoleLeft = 0;
        }

        // Activate Daemon Mode
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
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            if (isFile(e.FullPath))
            {
                printOverwrite(destName);
                copyFile(e.FullPath, destName, true);
            }
        }

        // Created callback
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            if (isFile(e.FullPath))
            {
                printCopy(destName);
                copyFile(e.FullPath, destName, false);
            }
            else
            {
                printMakeDir(destName);
                createDirectory(destName);
            }
        }

        // Renamed callback
        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            string oldDestName = destination + e.OldFullPath.Substring(source.Length);
            string newDestName = destination + e.FullPath.Substring(source.Length);
            printRename(oldDestName, newDestName);
            if (isFile(e.FullPath))
            {
                renameFile(oldDestName, newDestName);
            }
            else
            {
                renameDirectory(oldDestName, newDestName);
            }
        }

        // Deleted callback
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string destName = destination + e.FullPath.Substring(source.Length);
            printDelete(destName);
            if (isFile(destName))
            {
                deleteFile(destName);
            }
            else
            {
                deleteDirectory(destName);
            }
        }

        // Error callback
        private static void OnError(object sender, ErrorEventArgs e)
        {
            throw new Exception(e.GetException().Message);
        }

        // Check for file or dir
        private static bool isFile(string name)
        {
            FileAttributes attr = File.GetAttributes(name);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return false;
            }
            return true;
        }

        // Make Directory
        private static void createDirectory(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::makeDirectory(): [ERROR] {e.Message}");
            }
        }

        // Rename Directory
        private static void renameDirectory(string oldDir, string newDir)
        {
            try
            {
                Directory.Move(oldDir, newDir);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::renameDirectory(): [ERROR] {e.Message}");
            }
        }

        // Delete Directory
        private static void deleteDirectory(string dir)
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
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::deleteDirectory(): [ERROR] {e.Message}");
            }
        }

        // Copy File
        private static void copyFile(string srcFile, string destFile, bool overwrite)
        {
            try
            {
                File.Copy(srcFile, destFile, overwrite);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::copyFile(): [ERROR] {e.Message}");
            }
        }

        // Update File
        private static void updateFile(string srcFile, string destFile)
        {
            string backupFile = destFile + ".bac";
            try
            {
                File.Replace(srcFile, destFile, backupFile);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::updateFile(): [ERROR] {e.Message}");
            }
        }

        // Rename File
        private static void renameFile(string oldFile, string newFile)
        {
            try
            {
                File.Move(oldFile, newFile);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::renameFile(): [ERROR] {e.Message}");
            }
        }

        // Delete File
        private static void deleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::deleteFile(): [ERROR] {e.Message}");
            }
        }

        // Reset Cursor Position
        private static void resetCursor()
        {
            consoleTop = Console.GetCursorPosition().Top;
            Console.SetCursorPosition(consoleLeft, consoleTop);
        }

        // Print Copy
        private static void printCopy(string file)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[CP] ");
            printItemName(file);
        }

        // Print Make Directory
        private static void printMakeDir(string dir)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[MK DIR] ");
            printItemName(dir);
        }

        // Print Overwrite
        private static void printOverwrite(string file)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[OVERWRITE] ");
            printItemName(file);
        }

        // Print Rename
        private static void printRename(string oldName, string newName)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[RENAME] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{oldName} -> {newName}");
        }

        // Print Delete
        private static void printDelete(string name)
        {
            resetCursor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[DEL] ");
            printItemName(name);
        }

        // Print File Name
        private static void printItemName(string item)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{item}");
        }
    }
}
