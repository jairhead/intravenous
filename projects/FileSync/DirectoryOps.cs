// DirectoryOps.cs
// Contains the DirectoryOps class

namespace FileUtils
{
    public static class DirectoryOps
    {
        // Members
        private static object __directoryLock = new object();

        // Check if name is directory
        public static bool IsDirectory(string name)
        {
            lock (__directoryLock)
            {
                try
                {
                    if (Directory.Exists(name))
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
            return false;
        }

        // Make directory
        public static void CreateDirectory(string dir)
        {
            lock (__directoryLock)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
        }

        // Rename directory
        public static void RenameDirectory(string oldDir, string newDir)
        {
            lock (__directoryLock)
            {
                try
                {
                    Directory.Move(oldDir, newDir);
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
        }
        
        // Delete directory
        public static void DeleteDirectory(string dir)
        {
            lock (__directoryLock)
            {
                DirectoryInfo directory = new DirectoryInfo(dir);
                DirectoryInfo[] subdirectories = directory.GetDirectories("*.*", SearchOption.AllDirectories);
                FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);

                try
                {
                    foreach (FileInfo file in files)
                    {
                        FileOps.DeleteFile(file.FullName);
                    }

                    foreach (DirectoryInfo subdirectory in subdirectories)
                    {
                        Directory.Delete(subdirectory.FullName);
                    }

                    Directory.Delete(dir);
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
        }
    }
}