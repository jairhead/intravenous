// FileSync Main
using System;
using System.Formats.Asn1;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using ArgumentParsers;
using FileUtils;

class FileSync
{
    // Globals
    static bool copy = true;
    static bool sync = false;
    static bool replicate = false;
    static bool runDaemon = false;
    static int daemonMode = 1;
    static List<string> searchPatterns = new List<string>();

    static ConsoleColor defaultColor = Console.ForegroundColor;

    // Main
    static void Main(string[] args)
    {
        // Setup
        ConsoleColor defaultColor = Console.ForegroundColor;
        printBanner();
        Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelHandler);

        // Gather Input Args
        FileSyncArgumentParser parser = new FileSyncArgumentParser(args, "cdsrf:");
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

        // Activate daemon mode
        if (runDaemon)
        {
            SyncMonitor fm = new SyncMonitor(parser.getSrc(), parser.getDest(), daemonMode);
            fm.activateDaemonMode();

            while (true)
            {
                Thread.Sleep(30000);
            }
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
            daemonMode = 1;
        }

        if (parser.hasArg("-d"))
        {
            runDaemon = true;
        }

        if (parser.hasArg("-s"))
        {
            copy = false;
            sync = true;
            replicate = false;
            daemonMode = 2;
        }

        if (parser.hasArg("-r"))
        {
            copy = false;
            sync = false;
            replicate = true;
            daemonMode = 3;
        }

        if (parser.hasArg("-f") && copy)
        {
            Dictionary<string, string> providedPatterns = parser.getArgs();
            foreach (KeyValuePair<string, string> pattern in providedPatterns)
            {
                if (pattern.Key.Contains("-f"))
                {
                    searchPatterns.Add(pattern.Value);
                }
            }
        }

        return;
    }

    // Interrupt handler
    static void cancelHandler(object sender, ConsoleCancelEventArgs args)
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
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@%%%%@@@@@@@@@@@%%%%@@@@%%%@@@%%%%%%%%%%%%%%%%@@@@@@@%%%##%%@@@@@@%%###%%@@@@@@@@@@@@%%####%@@@@@@@%##%@@@@@@%##**##%@@@@@@@@@@@@@@",
            "@@@@@@@@@@%%%%%@@@@@@@@@%%%%%@@@@%%%%@@%###############@@@@%##########*##***********#@@@@@@@++++++++++++%@@@#==+@@@@#=======----=%@@@@@@@@@@",
            "@@@@@@@@@@%%%%%%@@@@@@@%%%%%%@@@@%%%%@@@@@@@@%###@@@@@@@@%####%@@@@@%##***#%@@@@@%**++%@@@%+++#@@@@@%#++#@@@#==+@@@#===%@@@@@#=--%@@@@@@@@@@",
            "@@@@@@@@@@%%%%%%%@@@@@%%%%%%%@@@@%%%%@@@@@@@@%###@@@@@@@@###%@@@@@@@@@@%%@@@@@@@@@@%+++%@@%+++@@@@@@@@@@@@@@#==+@@@+==*@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@%%%@%%%%@@@%%%%%%%%@@@@%%%%@@@@@@@@%###@@@@@@@###%@@@@@@@@@@@@@@@@@@@@@@@@%++*@@@*++++*#%%%@@@@@@@#==+@@@%====+*##%@@@@@@@@@@@@@@@",
            "@@@@@@@@@@%%%@@%%%@@%%%%@%%%%@@@@%%%%@@@@@@@@%###@@@@@@@###%@@@@@@@@@@@@@@@@@@@@@@@@@*++@@@@@*+++++++++*@@@@#==+@@@@@%======----%@@@@@@@@@@@",
            "@@@@@@@@@@%%%@@@%%%%%%%@@%%%%@@@@%%%%@@@@@@@@%###@@@@@@@###%@@@@@@@@@@@@@@@@@@@@@@@@%++*@@@@@@@@@@@%#++++@@@#==+@@@@@@@@@@@%%+---*@@@@@@@@@@",
            "@@@@@@@@@@%%%@@@@%%%%%@@@%%%%@@@@%%%%@@@@@@@@%###@@@@@@@@###%@@@@@@@@@@%%@@@@@@@@@@%+++%@@@%%@@@@@@@@@+++%@@#==+@@@@%@@@@@@@@@#---%@@@@@@@@@",
            "@@@@@@@@@@%%%@@@@@%%%@@@@%%%%@@@@%%%%@@@@@@@@%###@@@@@@@@%####%@@@@@%##***#%@@@@@%***+%@@@*+++#@@@@@@#+++@@@#==+@@%===+%@@@@@%=--*@@@@@@@@@@",
            "@@@@@@@@@@%%%@@@@@@%@@@@@%%%%@@@@%%%%@@@@@@@@%###@@@@@@@@@@%#############***********#@@@@@@%+++++++++++*@@@@#==+@@@@+=======----#@@@@@@@@@@@",
            "@@@@@@@@@@%%%@@@@@@@@@@@@@%%%@@@@%%%@@@@@@@@@%%%%@@@@@@@@@@@@@%%%##%%@@@@@@%%###%%@@@@@@@@@@@@%###*##%@@@@@@%##%@@@@@@@%#*++##@@@@@@@@@@@@@@",
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@",
            "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"
        };

        foreach (string line in banner)
        {
            Console.WriteLine(line);
            Thread.Sleep(20);
        }
    }
}
