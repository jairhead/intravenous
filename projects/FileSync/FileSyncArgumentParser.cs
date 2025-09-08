// FileSyncArgumentParser.cs
// Contains the FileSyncArgumentParser class
using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using FileUtils;
namespace ArgumentParsers
{
    public class FileSyncArgumentParser : BaseArgumentParser
    {
        // Source and destination directories
        protected string? src = null;
        protected string? dest = null;

        // Base Constructor
        public FileSyncArgumentParser(string[] inputArgs, string whiteList) : base(inputArgs, whiteList)
        {

        }

        // Overloadable extractArgs method
        public void parseArgs()
        {
            base.parseArgs();

            if (help == true)
            {
                return;
            }
            else if (input.Length < 2)
            {
                throw new ArgumentException("Source and destination directories must be provided!");
            }
            else if (!DirectoryOps.IsDirectory(input[input.Length - 2]))
            {
                throw new ArgumentException("Destination directory must be provided!");
            }
            else if (!DirectoryOps.IsDirectory(input[input.Length - 1]))
            {
                throw new ArgumentException("Source directory must be provided!");
            }

            try
            {
                setSrc(input[input.Length - 2]);
                setDest(input[input.Length - 1]);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Source directory setter
        private void setSrc(string dir)
        {
            if (Directory.Exists(dir))
            {
                src = dir;
                return;
            }
            throw new DirectoryNotFoundException("Source directory is invalid!");
        }

        // Destination directory setter
        private void setDest(string dir)
        {
            if (Directory.Exists(dir))
            {
                dest = dir;
                return;
            }
            throw new DirectoryNotFoundException("Destination directory is invalid!");
        }

        // Source directory getter
        public string getSrc()
        {
            if (src != null)
            {
                return src;
            }
            throw new NullReferenceException("Source directory is null!");
        }

        // Destination directory getter
        public string getDest()
        {
            if (dest != null)
            {
                return dest;
            }
            throw new NullReferenceException("Destination directory is null!");
        }
    }
}
