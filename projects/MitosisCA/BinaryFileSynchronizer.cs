// BinaryFileSynchronizer.cs
// Contains the BinaryFileSynchronizer class
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Principal;
using Microsoft.VisualBasic;

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
                if (!src.EndsWith('/'))
                {
                    src += "/";
                }

                if (!dest.EndsWith('/'))
                {
                    dest += "/";
                }

                this.src = new DirectoryInfo(src);
                this.dest = new DirectoryInfo(dest);

                ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[INDEXING SRC DIRECTORY] ",
                                                ConsoleColor.Gray, $"{this.src.FullName}");
                srcDirs = this.src.GetDirectories("*.*", SearchOption.AllDirectories);
                sortDirs(srcDirs);

                ConsoleLogger.WriteLine(ConsoleColor.Cyan, "[INDEXING SRC FILES]");
                srcFiles = this.src.GetFiles("*.*", SearchOption.AllDirectories);
                sortFiles(srcFiles);

                ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[INDEXING DEST DIRECTORY] ",
                                                ConsoleColor.Gray, $"{this.dest.FullName}");
                destDirs = this.dest.GetDirectories("*.*", SearchOption.AllDirectories);
                sortDirs(destDirs);

                ConsoleLogger.WriteLine(ConsoleColor.Cyan, "[INDEXING DEST FILES]");
                destFiles = this.dest.GetFiles("*.*", SearchOption.AllDirectories);
                sortFiles(destFiles);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Synchronize files src <-> dest
        public override void synchronize()
        {
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[START SYNC] ",
                                            ConsoleColor.Gray, $"{src.FullName} <-> {dest.FullName}");

            copyForward();
            copyBackward();

            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[FINISH SYNC] ",
                                            ConsoleColor.Gray, $"{src.FullName} <-> {dest.FullName}");
        }

        // Copy files src -> dest
        public override void copy()
        {
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[START COPY] ",
                                            ConsoleColor.Gray, $"{src.FullName} --> {dest.FullName}");

            copyForward();

            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[FINISH COPY] ",
                                            ConsoleColor.Gray, $"{src.FullName} --> {dest.FullName}");
        }

        // Replicate (destructive copy) src -> dest
        public override void replicate()
        {
            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[START REPLICATE] ",
                                            ConsoleColor.Gray, $"{src.FullName} --> {dest.FullName}");

            copyForward();
            deleteBackward();

            ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[FINISH REPLICATE] ",
                                            ConsoleColor.Gray, $"{src.FullName} --> {dest.FullName}");
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
                    //printMakeDir(destDirName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Green, "[MKDIR] ", ConsoleColor.Gray, $"{destDirName}");
                    DirectoryOps.CreateDirectory(destDirName);
                }
            }

            // Copy files
            foreach (FileInfo file in srcFiles)
            {
                string destFileName = dest.FullName + file.FullName.Substring(src.FullName.Length);
                if (!containsFile(destFiles, destFileName))
                {
                    //printCopy(destFileName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Green, "[CP] ", ConsoleColor.Gray, $"{destFileName}");
                    FileOps.CopyFile(file.FullName, destFileName, false);
                }
                else if (DateTime.Compare(File.GetLastWriteTime(file.FullName), File.GetLastWriteTime(destFileName)) > 0)
                {
                    //printOverwrite(destFileName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Yellow, "[OVRWRT] ", ConsoleColor.Gray, $"{destFileName}");
                    FileOps.CopyFile(file.FullName, destFileName, true);
                }
            }
        }

        // Copy files dest -> src
        private void copyBackward()
        {
            // Return immediately if there is nothing to copy
            if (destFiles.Length == 0)
            {
                return;
            }

            // Create any missing directories
            foreach (DirectoryInfo dir in destDirs)
            {
                string srcDirName = src.FullName + dir.FullName.Substring(dest.FullName.Length);
                if (!containsDir(srcDirs, srcDirName))
                {
                    //printMakeDir(srcDirName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Green, "[MKDIR] ", ConsoleColor.Gray, $"{srcDirName}");
                    DirectoryOps.CreateDirectory(srcDirName);
                }
            }

            // Copy files
            foreach (FileInfo file in destFiles)
            {
                string srcFileName = src.FullName + file.FullName.Substring(dest.FullName.Length);
                if (!containsFile(srcFiles, srcFileName))
                {
                    //printCopy(srcFileName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Green, "[CP] ", ConsoleColor.Gray, $"{srcFileName}");
                    FileOps.CopyFile(file.FullName, srcFileName, false);
                }
                else if (DateTime.Compare(File.GetLastWriteTime(file.FullName), File.GetLastWriteTime(srcFileName)) > 0)
                {
                    //printOverwrite(srcFileName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Green, "[CP] ", ConsoleColor.Gray, $"{srcFileName}");
                    FileOps.CopyFile(file.FullName, srcFileName, true);
                }
            }
        }

        // Delete dirs and files to make dest resemble src
        private void deleteBackward()
        {
            // Delete any files in dest that aren't in src
            foreach (FileInfo file in destFiles)
            {
                string srcFileName = src.FullName + file.FullName.Substring(dest.FullName.Length);
                if (!containsFile(srcFiles, srcFileName))
                {
                    //printDelete(file.FullName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Red, "[DEL] ", ConsoleColor.Gray, $"{file.FullName}");
                    FileOps.DeleteFile(file.FullName);
                }
            }

            // Delete any directories in dest that aren't in src
            for (int i = destDirs.Length - 1; i >= 0; i--)
            {
                DirectoryInfo dir = destDirs[i];
                string srcDirName = src.FullName + dir.FullName.Substring(dest.FullName.Length);
                if (!containsDir(srcDirs, srcDirName))
                {
                    //printDelete(dir.FullName);
                    ConsoleLogger.WriteTwoColorLine(ConsoleColor.Red, "[RMDIR] ", ConsoleColor.Gray, $"{dir.FullName}");
                    DirectoryOps.DeleteDirectory(dir.FullName);
                }
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
        private FileInfo? binFSearch(FileInfo[] files, string fileName, int begin, int end)
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
        private FileInfo? binFSearch(FileInfo[] files, string fileName)
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
        private DirectoryInfo? binDSearch(DirectoryInfo[] dirs, string name, int begin, int end)
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
        private DirectoryInfo? binDSearch(DirectoryInfo[] dirs, string name)
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
