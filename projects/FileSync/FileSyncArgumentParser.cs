// ArgumentParser.cs
// Contains the ArgumentParser class
using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace ArgumentParsers
{
    public class FileSyncArgumentParser : BaseArgumentParser
    {
        // Source and destination directories
        protected string src;
        protected string dest;

        // Base Constructor
        public FileSyncArgumentParser(string[] inputArgs, string whiteList) : base(inputArgs, whiteList)
        {
            try
            {
                extractArgs(inputArgs, whiteList);
            }
            catch
            {
                throw;
            }
        }

        // Overloadable extractArgs method
        private void extractArgs(string[] inputArgs, string whiteList)
        {
            // Immediately return if no args
            if (inputArgs.Length == 0)
            {
                return;
            }
            
            // Call super
            base.extractArgs(inputArgs, whiteList);

            // Set source dir
            if (!setSrc(inputArgs[inputArgs.Length - 2]))
            {
                throw new DirectoryNotFoundException("Source directory is invalid!");
            }

            // Set dest dir
            if (!setDest(inputArgs[inputArgs.Length - 1]))
            {
                throw new DirectoryNotFoundException("Destination directory is invalid!");
            }
        }

        // Source directory setter
        private bool setSrc(string dir)
        {
            if (Directory.Exists(dir))
            {
                src = dir;
                return true;
            }
            return false;
        }

        // Destination directory setter
        private bool setDest(string dir)
        {
            if (Directory.Exists(dir))
            {
                dest = dir;
                return true;
            }
            return false;
        }

        // Source directory getter
        public string getSrc()
        {
            return src;
        }

        // Destination directory getter
        public string getDest()
        {
            return dest;
        }
    }
}
