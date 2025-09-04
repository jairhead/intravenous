// HashFileSynchronizer.cs
// Contains the HashFileSynchronizer class
using System;
using System.Collections;
using System.ComponentModel;

namespace FileUtils {
    public class HashFileSynchronizer : AbstractFileSynchronizer
    {
        // Members
        Hashtable srcDirs = new Hashtable();
        Hashtable srcFiles = new Hashtable();

        Hashtable destDirs = new Hashtable();
        Hashtable destFiles = new Hashtable();

        // Constructor
        public HashFileSynchronizer(string src, string dest)
        {
            try
            {
                DirectoryInfo srcDir = new DirectoryInfo(src);
                DirectoryInfo[] srcDirs = srcDir.GetDirectories("*.*", SearchOption.AllDirectories);
                FileInfo[] srcFiles = srcDir.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (DirectoryInfo dir in srcDirs)
                {
                    this.srcDirs.Add(dir.FullName, dir);
                }

                foreach (FileInfo file in srcFiles)
                {
                    this.srcFiles.Add(file.FullName, file);
                }

                DirectoryInfo destDir = new DirectoryInfo(dest);
                DirectoryInfo[] destDirs = destDir.GetDirectories("*.*", SearchOption.AllDirectories);
                FileInfo[] destFiles = destDir.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (DirectoryInfo dir in destDirs)
                {
                    this.srcDirs.Add(dir.FullName, dir);
                }

                foreach (FileInfo file in destFiles)
                {
                    this.srcFiles.Add(file.FullName, file);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Synchronize files src <-> dest
        public override void synchronize()
        {

        }

        // Copy files src -> dest
        public override void copy()
        {

        }
        
        // Replicate (destructive copy) src -> dest
        public override void replicate()
        {
        }
    }
}
