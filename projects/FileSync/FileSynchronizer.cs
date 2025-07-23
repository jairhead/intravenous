// FileSynchronizer.cs
// Contains the FileSynchronizer class
using System;
using System.ComponentModel;

namespace FileUtils {
    public class FileSynchronizer
    {
        // Members
        DirectoryInfo src;
        DirectoryInfo[] srcDirs;
        FileInfo[] srcFiles;

        DirectoryInfo dest;
        DirectoryInfo[] destDirs;
        FileInfo[] destFiles;

        // Constructor
        public FileSynchronizer(string src, string dest)
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
        public void listSrcDirs()
        {
            Console.WriteLine("Source Directories:");
            foreach (DirectoryInfo dir in srcDirs)
            {
                Console.WriteLine(dir.Name);
            }
            Console.WriteLine();
        }

        // List Src Files
        public void listSrcFiles()
        {
            Console.WriteLine("Source Files:");
            foreach (FileInfo file in srcFiles)
            {
                Console.WriteLine(file.FullName);
            }
            Console.WriteLine();
        }

        // List Dest Dirs
        public void listDestDirs()
        {
            Console.WriteLine("Destination Directories:");
            foreach (DirectoryInfo dir in destDirs)
            {
                Console.WriteLine(dir.FullName);
            }
            Console.WriteLine();
        }

        // List Dest Files
        public void listDestFiles()
        {
            Console.WriteLine("Destionation Files:");
            foreach (FileInfo file in destFiles)
            {
                Console.WriteLine(file.Name);
            }
            Console.WriteLine();
        }

        // Synchronize files src <-> dest
        public void synchronize()
        {

        }

        // Copy files src -> dest
        public void copy()
        {

        }

        // Sort Files
        private void sortFiles()
        {
            try
            {
                Array.Sort(
                    srcFiles,
                    delegate (FileInfo f1, FileInfo f2)
                    {
                        return f1.FullName.CompareTo(f2.FullName);
                    }
                );

                Array.Sort(
                    destFiles,
                    delegate (FileInfo f1, FileInfo f2)
                    {
                        return f1.FullName.CompareTo(f2.FullName);
                    }
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Binary Search
        private int binSearch(FileInfo[] files, string name, int begin, int end)
        {
            if (end >= begin)
            {
                int middle = (begin + end) / 2;
                if (name == files[middle].FullName)
                {
                    return middle;
                }
                else if (name.CompareTo(files[middle].FullName) < 0)
                {
                    return binSearch(files, name, begin, middle);
                }
                else if (name.CompareTo(files[middle].FullName) > 0)
                {
                    return binSearch(files, name, (middle + 1), end);
                }
            }
            return -1;
        }
    }
}
