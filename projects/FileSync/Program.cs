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
            setFlags(parser);
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

    static void setFlags(FileSyncArgumentParser parser)
    {
        if (parser.hasArg("-c"))
        {
            fileSync = false;
            copyFiles = true;
        }

        if (parser.hasArg("-s"))
        {
            fileSync = true;
            copyFiles = false;
        }

        return;
    }
}
