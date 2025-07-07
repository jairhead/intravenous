// FileSync Main
using System;
using ArgumentParsers;

class FileSync
{
    // Globals
    bool fileSync = true;
    bool copyFiles = false;

    // Main
    static void Main(string[] args)
    {
        // Entry
        Console.WriteLine("FileSync::Main(): Start");

        // Gather Input Args
        FileSyncArgumentParser parser;
        Dictionary<string, string> arguments;
        try
        {
            parser = new FileSyncArgumentParser(args, "cs");
            arguments = parser.getArgs();
            setFlags(arguments);
        }
        catch (Exception e)
        {
            Console.WriteLine($"FileSync::Main(): [ERROR] {e.Message}");
            Console.WriteLine("FileSync::Main(): End");
            Environment.Exit(1);
        }

        // II.) Begin File Sync Threads

        // III.) Delete

        // Exit
        Console.WriteLine("FileSync::Main(): End");
        Environment.Exit(0);
    }

    static void setFlags(Dictionary<string,string> arguments)
    {
        foreach (KeyValuePair<string, string> entry in arguments)
        {
            Console.WriteLine($"Entry is {entry.ToString}");
        }
        return;
    }
}
