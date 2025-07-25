// BinaryFileSynchronizer.cs
// Contains the BinaryFileSynchronizer class
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security.Authentication;

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
                sortDirs(srcDirs);
                sortFiles(srcFiles);

                this.dest = new DirectoryInfo(dest);
                destDirs = this.dest.GetDirectories("*.*", SearchOption.AllDirectories);
                destFiles = this.dest.GetFiles("*.*", SearchOption.AllDirectories);
                sortDirs(destDirs);
                sortFiles(destFiles);
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
            Console.WriteLine("Destination Files:");
            foreach (FileInfo file in destFiles)
            {
                Console.WriteLine(file.FullName.Remove(0, dest.FullName.Length));
            }
            Console.WriteLine();
        }

        // Synchronize files src <-> dest
        public override void synchronize()
        {
            Console.WriteLine($"Synchronize {src.FullName} <-> {dest.FullName}");
            copyForward();
            copyBackward();
        }

        // Copy files src -> dest
        public override void copy()
        {
            Console.WriteLine($"Copy {src.FullName} -> {dest.FullName}");
            copyForward();
        }

        // Copy files src -> dest
        private void copyForward()
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
                    Console.WriteLine($"[MAKE DIR] {destDirName}");
                    makeDirectory(destDirName);
                }
            }

            // Copy files
            foreach (FileInfo file in srcFiles)
            {
                string destFileName = dest.FullName + file.FullName.Substring(src.FullName.Length);
                if (!containsFile(destFiles, destFileName))
                {
                    Console.WriteLine($"[COPY] {file} -> {destFileName}");
                    copyFile(file, destFileName, false);
                }
                else if (DateTime.Compare(File.GetLastWriteTime(file.FullName), File.GetLastWriteTime(destFileName)) > 0)
                {
                    Console.WriteLine($"[OVERWRITE] {destFileName}");
                    copyFile(file, destFileName, true);
                }
            }
        }

        // Copy files dest -> src
        private void copyBackward()
        {
            // Return immediately if there is nothing to copy
            if (srcFiles.Length == 0)
            {
                return;
            }

            // Create any missing directories
            foreach (DirectoryInfo dir in destDirs)
            {
                string srcDirName = src.FullName + dir.FullName.Substring(dest.FullName.Length);
                if (!containsDir(srcDirs, srcDirName))
                {
                    Console.WriteLine($"[MAKE DIR] {srcDirName}");
                    makeDirectory(srcDirName);
                }
            }

            // Copy files
            foreach (FileInfo file in destFiles)
            {
                string srcFileName = src.FullName + file.FullName.Substring(dest.FullName.Length);
                if (!containsFile(srcFiles, srcFileName))
                {
                    Console.WriteLine($"[COPY] {file} -> {srcFileName}");
                    copyFile(file, srcFileName, false);
                }
                else if (DateTime.Compare(File.GetLastWriteTime(file.FullName), File.GetLastWriteTime(srcFileName)) > 0)
                {
                    Console.WriteLine($"[OVERWRITE] {srcFileName}");
                    copyFile(file, srcFileName, true);
                }
            }
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
                Console.WriteLine($"BinaryFileSynchronizer::copyFile(): [ERROR] {e.Message}");
            }
        }

        // Sort Directories
        private void sortDirs(DirectoryInfo[] dirs)
        {
            try
            {
                Array.Sort(dirs, delegate (DirectoryInfo d1, DirectoryInfo d2) { return d1.FullName.CompareTo(d2.FullName); });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Sort Files
        private void sortFiles(FileInfo[] files)
        {
            try
            {
                Array.Sort(files, delegate (FileInfo f1, FileInfo f2) { return f1.FullName.CompareTo(f2.FullName); });
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
    }
}
