// FileSync Main
using System;
using ArgumentParsers;

class FileSync
{
    static void Main(string[] args)
    {
        // Entry
        Console.WriteLine("FileSync::Main(): Start");

        // I.) Gather Input Args
        FileSyncArgumentParser parser;
        try
        {
            parser = new FileSyncArgumentParser(args, "abcd:ef:gh");
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine($"FileSync::Main(): {e.Message}");
        }

        // II.) Begin File Sync Threads

        // III.) Delete

        // Exit
        Console.WriteLine("FileSync::Main(): End");
        Environment.Exit(0);
    }
}
