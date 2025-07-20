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

        // List Source Dirs
        public void listSrc()
        {
            DirectoryInfo[] srcDirs = src.GetDirectories("*.*", SearchOption.AllDirectories);
            Console.WriteLine("Printing subdirectories of src");
            foreach (DirectoryInfo dir in srcDirs)
            {
                Console.WriteLine(dir.Name);
            }
        }

        // List Dest Dirs
        public void listDest()
        {
            DirectoryInfo[] destDirs = dest.GetDirectories("*.*", SearchOption.AllDirectories);
            Console.WriteLine("Printing subdirectories of dest");
            foreach (DirectoryInfo dir in destDirs)
            {
                Console.WriteLine(dir.Name);
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
