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

            // Otherwise, extract arguments
            for (int i = 0; i < inputArgs.Length - 2; i++)
            {
                if (inputArgs[i].Contains('-') && whiteList.Contains(inputArgs[i].Remove(0, 1)) &&
                    whiteList.Contains(inputArgs[i].Remove(0, 1) + ":") && !args.ContainsKey(inputArgs[i]) &&
                    checkArgVal(inputArgs, i))
                {
                    args.Add(inputArgs[i], inputArgs[i + 1]);
                    i++;
                }
                else if (inputArgs[i].Contains('-') && whiteList.Contains(inputArgs[i].Remove(0, 1)) &&
                         !args.ContainsKey(inputArgs[i]))
                {
                    args.Add(inputArgs[i], "NA");
                }
                else
                {
                    continue;
                }
            }

            // Set source dir
            if (!setSrc(inputArgs))
            {
                throw new DirectoryNotFoundException("Source directory is invalid!");
            }

            // Set dest dir
            if (!setDest(inputArgs))
            {
                throw new DirectoryNotFoundException("Destination directory is invalid!");
            }
        }

        // Source directory setter
        private bool setSrc(string[] inputArgs)
        {
            if (Directory.Exists(inputArgs[inputArgs.Length - 2]))
            {
                src = inputArgs[inputArgs.Length - 2];
                return true;
            }
            return false;
        }

        // Destination directory setter
        private bool setDest(string[] inputArgs)
        {
            if (Directory.Exists(inputArgs[inputArgs.Length - 1]))
            {
                dest = inputArgs[inputArgs.Length - 1];
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
