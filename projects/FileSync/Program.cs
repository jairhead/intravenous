// FileSync Main
using System;
using ArgumentParsers;

class FileSync
{
    static void Main(string[] args)
    {
        // Entry statement
        Console.WriteLine("FileSync::Main(): Start");

        // I.) Gather Input Args
        BaseArgumentParser parser = new BaseArgumentParser(args, "abcd:ef:gh");

        // II.) Begin File Sync Threads

        // III.) Delete

        // Exit statement
        Console.WriteLine("FileSync::Main(): End");
        Environment.Exit(0);
    }
}
