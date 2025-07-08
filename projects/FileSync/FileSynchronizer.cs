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

        // List
        public void list()
        {
            DirectoryInfo[] srcDi = src.GetDirectories();
            Console.WriteLine("Printing subdirectories of src");
            foreach (DirectoryInfo dri in srcDi)
            {
                Console.WriteLine(dri.Name);
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
