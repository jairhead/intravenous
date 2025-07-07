// BaseArgumentParser.cs
// Contains the BaseArgumentParser class
using System;
using System.Runtime.InteropServices;

namespace ArgumentParsers
{
    public class BaseArgumentParser
    {
        //  Internal variables
        protected Dictionary<string, string> args = new Dictionary<string, string>();
        protected string[] input;
        protected string allowedArgs;


        // Constructor
        public BaseArgumentParser(string[] inputArgs, string whiteList)
        {
            input = inputArgs;
            allowedArgs = whiteList;
        }

        // Overloadable extractArgs method
        public void parseArgs()
        {
            if (input.Length == 0)
            {
                return;
            }

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Contains('-') && allowedArgs.Contains(input[i].Remove(0, 1)) &&
                    allowedArgs.Contains(input[i].Remove(0, 1) + ":") &&
                    checkArgVal(input, i))
                {
                    int count = countOccurrences(input[i]);
                    if (count >= 1)
                    {
                        args.Add(input[i] + count.ToString(), input[i + 1]);
                    }
                    else
                    {
                        args.Add(input[i], input[i + 1]);
                    }
                    i++;
                }
                else if (input[i].Contains('-') && allowedArgs.Contains(input[i].Remove(0, 1)) &&
                         !args.ContainsKey(input[i]))
                {
                    args.Add(input[i], "-");
                }
            }
        }

        // Checks to see if a value was provided
        protected bool checkArgVal(string[] inputArgs, int i)
        {
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

        // Count number of times an argument has been stored
        protected int countOccurrences(string key)
        {
            int count = 0;
            foreach (KeyValuePair<string, string> entry in args)
            {
                if (entry.Key.Contains(key))
                {
                    count++;
                }
            }
            return count;
        }

        // Return the arguments extracted
        public Dictionary<string, string> getArgs()
        {
            return args;
        }

        // Check for argument
        public bool hasArg(string key)
        {
            return args.ContainsKey(key);
        }

        // Get the value of the key
        public string getVal(string key)
        {
            return args[key];
        }
    }
}
