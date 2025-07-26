// FileSync Main
using System;
using System.IO;
using ArgumentParsers;
using FileUtils;

class FileSync
{
    // Globals
    static bool copy = true;
    static bool sync = false;
    static bool replicate = false;

    static ConsoleColor defaultColor = Console.ForegroundColor;

    // Main
    static void Main(string[] args)
    {
        // Setup
        ConsoleColor defaultColor = Console.ForegroundColor;
        printBanner();
        Console.CancelKeyPress += new ConsoleCancelEventHandler(handler);

        // Gather Input Args
        FileSyncArgumentParser parser = new FileSyncArgumentParser(args, "csr");
        try
        {
            parser.parseArgs();
            setFlags(parser);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = defaultColor;
            Console.WriteLine($"FileSync::Main(): [ERROR] {e.Message}");
            Environment.Exit(1);
        }

        // Perform Specified Operation
        BinaryFileSynchronizer fs = new BinaryFileSynchronizer(parser.getSrc(), parser.getDest());
        try
        {
            if (copy)
            {
                fs.copy();
            }
            else if (sync)
            {
                fs.synchronize();
            }
            else if (replicate)
            {
                fs.replicate();
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = defaultColor;
            Console.WriteLine($"FileSync::Main(): [ERROR] {e.Message}");
            Environment.Exit(1);
        }

        // Exit
        Environment.Exit(0);
    }

    // Helper method for setting program flags
    static void setFlags(FileSyncArgumentParser parser)
    {
        if (parser.hasArg("-c"))
        {
            copy = true;
            sync = false;
            replicate = false;
        }

        if (parser.hasArg("-s"))
        {
            copy = false;
            sync = true;
            replicate = false;
        }

        if (parser.hasArg("-r"))
        {
            copy = false;
            sync = false;
            replicate = true;
        }

        return;
    }

    // Interrupt handler
    static void handler(object sender, ConsoleCancelEventArgs args)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[INTERRUPT] ");
        Console.WriteLine("Exiting program.");
        Console.ForegroundColor = defaultColor;
        Environment.Exit(1);
    }

    // Print the banner for the program
    static void printBanner()
    {
        List<string> banner = new List<string>
        {
            "                                                                              ",
            " |█▄       ▄█ |███████ |███████   .▄███▄  .▄███▄    .▄███▄  |███████  .▄███▄  ",
            " |██▄     ▄██    |█       |█     |█▀   ▀█▄█▀   ▀█  |█▀   ▀█    |█    |█▀   ▀█ ",
            " |███▄   ▄███    |█       |█    |█      |█      |█ |█▄         |█    |█▄      ",
            " |██|█▄ ▄█|██    |█       |█    |█              |█   ▀███▄     |█      ▀███▄  ",
            " |██ |███ |██    |█       |█    |█      |█      |█       ▀█    |█          ▀█ ",
            " |██  |█  |██    |█       |█     |█▄.  ▄█▀█▄  .▄█  |█▄  .▄█    |█    |█▄  .▄█ ",
            " |██      |██ |███████    |█       ▀███▀   ▀███▀     ▀███▀  |███████   ▀███▀  ",
            "                                                                              "
        };

        foreach (string line in banner)
        {
            Console.WriteLine(line);
        }
    }
}
