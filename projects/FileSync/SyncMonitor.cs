// SyncMonitor.cs
// Contains the SyncMonitor class

using System.IO.IsolatedStorage;

namespace FileUtils
{
    public class SyncMonitor
    {
        // Members
        static string source = "";
        FileSystemWatcher srcWatcher;

        static string destination = "";

        private int consoleLeft;
        private int consoleTop;

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
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        // Created callback
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Created: {e.FullPath}");
        }

        // Renamed callback
        private static void OnRenamed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Renamed: {e.FullPath}");
        }

        // Deleted callback
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");
        }

        // Error callback
        private static void OnError(object sender, ErrorEventArgs e)
        {
            throw new Exception(e.GetException().Message);
        }

        // Check for file or dir
        private static bool isFile(FileSystemEventArgs e)
        {
            FileAttributes attr = File.GetAttributes(e.FullPath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return false;
            }
            return true;
        }

        // Make Directory
        private void makeDirectory(string dirName)
        {
            try
            {
                Directory.CreateDirectory(dirName);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::copy(): [ERROR] {e.Message}");
            }
        }

        // Delete Directory
        private void deleteDirectory(string dirName)
        {
            try
            {
                Directory.Delete(dirName);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::copy(): [ERROR] {e.Message}");
            }
        }

        // Copy File
        private void copyFile(FileInfo file, string fileName, bool overwrite)
        {
            try
            {
                file.CopyTo(fileName, overwrite);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::copyFile(): [ERROR] {e.Message}");
            }
        }

        // Delete File
        private void deleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (UnauthorizedAccessException e)
            {
                resetCursor();
                Console.WriteLine($"BinaryFileSynchronizer::copyFile(): [ERROR] {e.Message}");
            }
        }

        // Reset Cursor Position
        private void resetCursor()
        {
            consoleTop = Console.GetCursorPosition().Top;
            Console.SetCursorPosition(consoleLeft, consoleTop);
        }
    }
}
