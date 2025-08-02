// SyncMonitor.cs
// Contains the SyncMonitor class

namespace FileUtils
{
    public class SyncMonitor
    {
        // Members
        string src;
        FileSystemWatcher srcWatcher;

        string dest;
        FileSystemWatcher destWatcher;

        int daemonMode = 1;
        ConsoleColor defaultColor = Console.ForegroundColor;

        // Constructor
        public SyncMonitor(string src, string dest, int daemonMode)
        {
            this.src = src;
            this.dest = dest;
            this.daemonMode = daemonMode;
        }

        // Activate Daemon Mode
        public void activateDaemonMode()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[ACTIVATE DAEMON]");
            Console.ForegroundColor = defaultColor;

            setUpSrcWatcher();
            setUpDestWatcher();
        }

        // Set up source watcher
        private void setUpSrcWatcher()
        {
            srcWatcher = new FileSystemWatcher(src);
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

        // Set up destination watcher
        private void setUpDestWatcher()
        {
            if (daemonMode == 2 || daemonMode == 3)
            {
                destWatcher = new FileSystemWatcher(dest);
                destWatcher.NotifyFilter = NotifyFilters.Attributes
                                        | NotifyFilters.CreationTime
                                        | NotifyFilters.DirectoryName
                                        | NotifyFilters.FileName
                                        | NotifyFilters.LastAccess
                                        | NotifyFilters.LastWrite
                                        | NotifyFilters.Security
                                        | NotifyFilters.Size;
                destWatcher.Changed += OnChanged;
                destWatcher.Renamed += OnRenamed;
                destWatcher.Deleted += OnDeleted;
                destWatcher.Error += OnError;
                destWatcher.IncludeSubdirectories = true;
                destWatcher.EnableRaisingEvents = true;

                if (daemonMode == 2)
                {
                    destWatcher.Created += OnCreated;
                }
                else
                {
                    destWatcher.Created += OnCreatedDestReplicate;
                }
            }
        }

        // Changed callback
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        // Created callback
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Created: {e.FullPath}");
        }

        private static void OnCreatedDestReplicate(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Replicate Created: {e.FullPath}");
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
    }
}