// FileSynchronizer.cs
// Contains the FileSynchronizer class
using System;
using System.ComponentModel;

namespace FileUtils {
    public class FileSynchronizer
    {
        // Internal variables
        DirectoryInfo src;
        DirectoryInfo dest;

        // Constructor
        public FileSynchronizer(string src, string dest)
        {
            this.src = new DirectoryInfo(src);
            this.dest = new DirectoryInfo(dest);
        }

        // List Src Dirs
        public void listSrcDirs()
        {
            DirectoryInfo[] srcDirs = src.GetDirectories("*.*", SearchOption.AllDirectories);
            Console.WriteLine("Printing subdirectories of src");
            foreach (DirectoryInfo dir in srcDirs)
            {
                Console.WriteLine(dir.Name);
            }
        }

        // List Src Files
        public void listSrcFiles()
        {
            FileInfo[] srcFiles = src.GetFiles("*.*", SearchOption.AllDirectories);
            Console.WriteLine("Printing subdirectories of src");
            foreach (FileInfo file in srcFiles)
            {
                Console.WriteLine(file.Name);
            }
        }

        // List Dest Dirs
        public void listDestDirs()
        {
            DirectoryInfo[] destDirs = dest.GetDirectories("*.*", SearchOption.AllDirectories);
            Console.WriteLine("Printing subdirectories of dest");
            foreach (DirectoryInfo dir in destDirs)
            {
                Console.WriteLine(dir.Name);
            }
        }

        // List Dest Files
        public void listDestFiles()
        {
            FileInfo[] destFiles = dest.GetFiles("*.*", SearchOption.AllDirectories);
            Console.WriteLine("Printing subdirectories of src");
            foreach (FileInfo file in destFiles)
            {
                Console.WriteLine(file.Name);
            }
        }

        // Synchronize files src <-> dest
        public void synchronize()
        {

        }

        // Copy files src -> dest
        public void copy()
        {
            
        }
    }
}
