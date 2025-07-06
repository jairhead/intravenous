// ArgumentParser.cs
// Contains the ArgumentParser class
using System;
using System.Runtime.InteropServices;

namespace ArgumentParsers
{
    public class BaseArgumentParser
    {
        //  Internal variables
        protected Dictionary<string, string> args = new Dictionary<string, string>();

        // Constructor
        public BaseArgumentParser(string[] inputArgs, string whiteList)
        {
            extractArgs(inputArgs, whiteList);
        }

        // Overloadable extractArgs method
        protected void extractArgs(string[] inputArgs, string whiteList)
        {
            // Immediately return if no args
            if (inputArgs.Length == 0)
            {
                return;
            }

            // Otherwise, extract arguments
            for (int i = 0; i < inputArgs.Length; i++)
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
        }

        // Checks to see if a value was provided
        protected bool checkArgVal(string[] inputArgs, int i)
        {
            // Handle scenario where no value is provided
            if (i + 1 >= inputArgs.Length)
            {
                return false;
            }
            else if (inputArgs[i + 1].Contains('-'))
            {
                return false;
            }
            return true;
        }

        // Return the arguments extracted
        public Dictionary<string, string> getArgs()
        {
            return args;
        }
    }
}
