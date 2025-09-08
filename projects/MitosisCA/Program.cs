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
        Console.CancelKeyPress += new ConsoleCancelEventHandler(cancel);
        banner();

        // Gather Input Args
        if (args.Length == 0)
        {
            usage();
            Environment.Exit(0);
        }

        FileSyncArgumentParser parser = new FileSyncArgumentParser(args, "cdhsr");

        try
        {
            parser.parseArgs();
            flags(parser);
        }
        catch (Exception e)
        {
            ConsoleLogger.Error(e);
            usage();
            Environment.Exit(1);
        }

        if (parser.needHelp())
        {
            usage();
            Environment.Exit(0);
        }

        // Perform Initial Operation
        DateTime start = DateTime.Now;
        ConsoleLogger.WriteTwoColorLine(ConsoleColor.Blue, "[START TIME] ", ConsoleColor.Gray, $"{start}");
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

        DateTime finish = DateTime.Now;
        ConsoleLogger.WriteTwoColorLine(ConsoleColor.Blue, "[FINISH TIME] ", ConsoleColor.Gray, $"{finish}");

        TimeSpan elapsed = finish - start;
        ConsoleLogger.WriteTwoColorLine(ConsoleColor.Blue, "[ELAPSED TIME] ", ConsoleColor.Gray, $"{elapsed}");

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
        s.stopGood();
        Environment.Exit(0);
    }

    // Helper method for setting program flags
    static void flags(FileSyncArgumentParser parser)
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

        return;
    }

    // Interrupt handler
    static void cancel(object sender, ConsoleCancelEventArgs args)
    {
        s.stopInterrupt();
        Environment.Exit(0);
    }

    // Print the banner for the program
    static void banner()
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
            Thread.Sleep(40);
        }
    }

    // Print usage
    static void usage()
    {
        List<string> usage = new List<string>
        {
            "Usage: MitosisCA.exe [-c | -s | -r] [-d] src dest",
            "  -c: copy files from src to dest                ",
            "  -s: synchronize src and dest (constructive)    ",
            "  -r: replicate src to dest (destructive copy)   ",
            "  -d: run daemon mode                            ",
            "  src: source folder / directory (required)      ",
            "  dest: destination folder / directory (required)"
        };

        foreach (string line in usage)
        {
            ConsoleLogger.WriteLine(line);
            Thread.Sleep(40);
        }
    }
}
