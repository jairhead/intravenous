// FileSync Main
using System;
using ArgumentParsers;

class FileSync
{
    // Globals
    static bool fileSync = true;
    static bool copyFiles = false;

    // Main
    static void Main(string[] args)
    {
        // Entry
        Console.WriteLine("FileSync::Main(): Start");

        // Gather Input Args
        FileSyncArgumentParser parser = new FileSyncArgumentParser(args, "gancs");
        Dictionary<string, string> arguments;
        try
        {
            parser.parseArgs();
            arguments = parser.getArgs();
            setFlags(arguments);
        }
        catch (Exception e)
        {
            Console.WriteLine($"FileSync::Main(): [ERROR] {e.Message}");
            Console.WriteLine("FileSync::Main(): End");
            Environment.Exit(1);
        }

        // Begin File Sync Threads
        

        // Exit
        Console.WriteLine("FileSync::Main(): End");
        Environment.Exit(0);
    }

    static void setFlags(Dictionary<string,string> arguments)
    {
        Console.WriteLine("Foo");
        foreach (KeyValuePair<string, string> entry in arguments)
        {
            switch (entry.Key)
            {
                case "-c":
                    fileSync = false;
                    copyFiles = true;
                    break;
                case "-s":
                    fileSync = true;
                    copyFiles = false;
                    break;
            }
        }
        return;
    }
}
