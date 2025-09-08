// FilesOps.cs
// Contains the FileOps class

namespace FileUtils
{
    public static class FileOps
    {
        // Members
        private static object __fileLock = new object();

        // Check if is file
        public static bool IsFile(string name)
        {
            lock (__fileLock)
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
                    ConsoleLogger.Error(e);
                }
            }
            return false;
        }

        // Copy file
        public static void CopyFile(string srcFile, string destFile, bool overwrite)
        {
            lock (__fileLock)
            {
                try
                {
                    File.Copy(srcFile, destFile, overwrite);
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
        }

        // Rename file
        public static void RenameFile(string oldFile, string newFile)
        {
            lock (__fileLock)
            {
                try
                {
                    File.Move(oldFile, newFile);
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
        }

        // Delete file
        public static void DeleteFile(string file)
        {
            lock (__fileLock)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
        }
    }
}