// FileSync Main
using System;
using System.Formats.Asn1;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using ArgumentParsers;
using FileUtils;
using TerminalUtils;

class FileSync
{
    // Globals
    static bool copy = true;
    static bool sync = false;
    static bool replicate = false;
    static bool runDaemon = false;
    static List<string> searchPatterns = new List<string>();
    static Spinner s = new Spinner();

    // Main
    static void Main(string[] args)
    {
        // Setup
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelHandler);
        printBanner();

        // Gather Input Args
        FileSyncArgumentParser parser = new FileSyncArgumentParser(args, "cdsrf:");
        try
        {
            parser.parseArgs();
            setFlags(parser);
        }
        catch (Exception e)
        {
            ConsoleLogger.Error(e);
            exit(1);
        }

        // Perform Initial Operation
        s.start();
        if (copy || sync || replicate)
        {
            try
            {
                BinaryFileSynchronizer fs = new BinaryFileSynchronizer(parser.getSrc(), parser.getDest());
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
                ConsoleLogger.Error(e);
            }
        }

        // Daemon Mode
        if (runDaemon)
        {
            try
            {
                SyncMonitor fm = new SyncMonitor(parser.getSrc(), parser.getDest());
                fm.activateDaemon();
            }
            catch (Exception e)
            {
                ConsoleLogger.Error(e);
            }

            while (true)
            {
                Thread.Sleep(30000);
            }
        }

        // Exit
        exit(0);
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

        if (parser.hasArg("-d"))
        {
            runDaemon = true;
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
        ConsoleLogger.WriteTwoColorLine(ConsoleColor.Cyan, "[INTERRUPT] ", ConsoleColor.Gray, "Exiting program.");
        exit(0);
    }

    // Print the banner for the program
    static void printBanner()
    {
        List<string> banner = new List<string>
        {
            "                                                                                                   ",
            "   ...         ..    ..   ............      .;;:.     .;;:.          .:;:.      ..      .:;:.      ",
            "  .&&&+      :&&&;  :$&; :$&&&&&&&&&&$:  +$&&&&&&&XX$&&&&&&&X;    .X&&&&&&&$+  .X&;   x&&&&&&&$x.  ",
            "  .&&&&;    :&&&&;  :$&;      :&$:     .$&&.     ;$&X:     ;&&x   X&+     :+;  .X&;  x&$.    .+;   ",
            "  .&$X&$:  .$&+&&;  :$&;      :&$:     X&X                  .$&x  X&$;         .X&;  +&&+.         ",
            "  .&$;+&$:.$&+ &&;  :$&;      :&$:    .$&:                   ;&X.  ;$&&&&&&x.  .X&;   :$&&&&&&X:   ",
            "  .&$: +&$&&x  &&;  :$&;      :&$:     X&x                  .X&x         .X&$; .X&;         .+&&;  ",
            "  .&$:  x&&x   &&;  :$&;      :&$:     ;$&x      .x$+      .X&$:  x+.     :$&+ .X&;  ;$.     .X&+  ",
            "  .&$:   xx    &&;  :$&;      :&$:       X&&&$X$&&&$&&&$X$&&&x    x&&&$xx&&&+  .X&;  +$&&$xx$&&X.  ",
            "   x+.         xx.  .+x.      .x+.         :xXXX+.   :xXXX+.        .+XXXx:     +x:     ;xXXx;     ",
            "                                                                                                   "
        };

        foreach (string line in banner)
        {
            ConsoleLogger.WriteLine(line);
            Thread.Sleep(20);
        }
    }

    // Exit
    static void exit(int code)
    {
        s.stop();
        Environment.Exit(code);
    }
}
