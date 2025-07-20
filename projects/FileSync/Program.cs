// FileSync Main
using System;
using System.IO;
using ArgumentParsers;
using FileUtils;

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
        FileSyncArgumentParser parser = new FileSyncArgumentParser(args, "cs");
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

        // Begin
        FileSynchronizer fs = new FileSynchronizer(parser.getSrc(), parser.getDest());
        if (fileSync)
        {
            fs.synchronize();
        }
        else
        {
            fs.copy();
        }

        // Exit
        Console.WriteLine("FileSync::Main(): End");
        Environment.Exit(0);
    }

    // Helper Method for Setting Flags
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
