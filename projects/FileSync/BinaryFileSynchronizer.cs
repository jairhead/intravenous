// BinaryFileSynchronizer.cs
// Contains the BinaryFileSynchronizer class
using System;
using System.ComponentModel;

namespace FileUtils {
    public class BinaryFileSynchronizer : AbstractFileSynchronizer
    {
        // Members
        DirectoryInfo src;
        DirectoryInfo[] srcDirs;
        FileInfo[] srcFiles;

        DirectoryInfo dest;
        DirectoryInfo[] destDirs;
        FileInfo[] destFiles;

        // Constructor
        public BinaryFileSynchronizer(string src, string dest)
        {
            try
            {
                this.src = new DirectoryInfo(src);
                srcDirs = this.src.GetDirectories("*.*", SearchOption.AllDirectories);
                srcFiles = this.src.GetFiles("*.*", SearchOption.AllDirectories);

                this.dest = new DirectoryInfo(dest);
                destDirs = this.dest.GetDirectories("*.*", SearchOption.AllDirectories);
                destFiles = this.dest.GetFiles("*.*", SearchOption.AllDirectories);

                sortFiles();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // List Src Dirs
        public override void listSrcDirs()
        {
            Console.WriteLine("Source Directories:");
            foreach (DirectoryInfo dir in srcDirs)
            {
                Console.WriteLine(dir.FullName.Remove(0, src.FullName.Length));
            }
            Console.WriteLine();
        }

        // List Src Files
        public override void listSrcFiles()
        {
            Console.WriteLine("Source Files:");
            foreach (FileInfo file in srcFiles)
            {
                Console.WriteLine(file.FullName.Remove(0, src.FullName.Length));
            }
            Console.WriteLine();
        }

        // List Dest Dirs
        public override void listDestDirs()
        {
            Console.WriteLine("Destination Directories:");
            foreach (DirectoryInfo dir in destDirs)
            {
                Console.WriteLine(dir.FullName.Remove(0, dest.FullName.Length));
            }
            Console.WriteLine();
        }

        // List Dest Files
        public override void listDestFiles()
        {
            Console.WriteLine("Destionation Files:");
            foreach (FileInfo file in destFiles)
            {
                Console.WriteLine(file.FullName.Remove(0, dest.FullName.Length));
            }
            Console.WriteLine();
        }

        // Synchronize files src <-> dest
        public override void synchronize()
        {

        }

        // Copy files src -> dest
        public override void copy()
        {
            // Return immediately if there is nothing to copy
            if (srcFiles.Length == 0)
            {
                return;
            }

            // Create any missing directories
            foreach (DirectoryInfo dir in srcDirs)
            {
                string destDirName = dest.FullName + dir.FullName.Substring(src.FullName.Length);
                if (!containsDir(destDirs, destDirName))
                {
                    try
                    {
                        Directory.CreateDirectory(destDirName);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"BinaryFileSynchronizer::copy(): [ERROR] {e.Message}");
                    }
                }
            }

            // Copy files
            foreach (FileInfo file in srcFiles)
            {
                string destFileName = dest.FullName + file.FullName.Substring(src.FullName.Length);
                if (!containsFile(destFiles, destFileName))
                {
                    file.CopyTo(destFileName, false);
                }
                else if (DateTime.Compare(File.GetLastWriteTime(file.FullName), File.GetLastWriteTime(destFileName)) > 0)
                {
                    file.CopyTo(destFileName, true);
                }
            }
        }

        // Sort Files
        private void sortFiles()
        {
            try
            {
                Array.Sort(srcFiles, delegate (FileInfo f1, FileInfo f2) { return f1.FullName.CompareTo(f2.FullName); });
                Array.Sort(destFiles, delegate (FileInfo f1, FileInfo f2) { return f1.FullName.CompareTo(f2.FullName); });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Binary File Search
        private FileInfo binFSearch(FileInfo[] files, string fileName, int begin, int end)
        {
            if (end > begin)
            {
                int middle = (begin + end) / 2;
                int compare = fileName.CompareTo(files[middle].FullName);
                if (compare == 0)
                {
                    return files[middle];
                }
                else if (compare < 0)
                {
                    return binFSearch(files, fileName, begin, middle);
                }
                else if (compare > 0)
                {
                    return binFSearch(files, fileName, (middle + 1), end);
                }
            }
            return null;
        }

        // Binary File Search (Overloaded)
        private FileInfo binFSearch(FileInfo[] files, string fileName)
        {
            return binFSearch(files, fileName, 0, files.Length);
        }

        // Contains File
        private bool containsFile(FileInfo[] files, string fileName)
        {
            if (binFSearch(files, fileName) != null)
            {
                return true;
            }
            return false;
        }

        // Binary Directory Search
        private DirectoryInfo binDSearch(DirectoryInfo[] dirs, string name, int begin, int end)
        {
            if (end > begin)
            {
                int middle = (begin + end) / 2;
                int compare = name.CompareTo(dirs[middle].FullName);
                if (compare == 0)
                {
                    return dirs[middle];
                }
                else if (compare < 0)
                {
                    return binDSearch(dirs, name, begin, middle);
                }
                else if (compare > 0)
                {
                    return binDSearch(dirs, name, (middle + 1), end);
                }
            }
            return null;
        }

        // Binary Directory Search (Overloaded)
        private DirectoryInfo binDSearch(DirectoryInfo[] dirs, string name)
        {
            return binDSearch(dirs, name, 0, dirs.Length);
        }

        // Contains Directory
        private bool containsDir(DirectoryInfo[] dirs, string name)
        {
            if (binDSearch(dirs, name) != null)
            {
                return true;
            }
            return false;
        }

        // Compare File Age
        private int compareFileAge(string f1, string f2)
        {
            int compare = DateTime.Compare(File.GetLastWriteTime(f1), File.GetLastWriteTime(f2));
            if (compare < 0)
            {
                Console.WriteLine($"{f1} was modified earlier than {f2}");
            }
            else if (compare > 0)
            {
                Console.WriteLine($"{f1} was modified later than {f2}");
            }
            else
            {
                Console.WriteLine($"{f1} and {f2} have the same time of last modification.");
            }
            return compare;
        }
    }
}
