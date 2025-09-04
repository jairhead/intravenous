// ConsoleLogger.cs
// Contains the ConsoleLogger class

using System.Security.Cryptography.X509Certificates;

namespace FileUtils
{
    public static class ConsoleLogger
    {
        // Members
        private static int consoleLeft = 0;
        private static int consoleTop;
        private static object __printLock = new object();

        // Write
        public static void Write(string text)
        {
            lock (__printLock)
            {
                resetCursor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(text);
            }
        }

        // Write (Overloaded)
        public static void Write(ConsoleColor color, string text)
        {
            lock (__printLock)
            {
                resetCursor();
                Console.ForegroundColor = color;
                Console.Write(text);
            }
        }

        // Write Line
        public static void WriteLine(string text)
        {
            lock (__printLock)
            {
                resetCursor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(text);
            }
        }

        // Write Line (Overloaded)
        public static void WriteLine(ConsoleColor color, string text)
        {
            lock (__printLock)
            {
                resetCursor();
                Console.ForegroundColor = color;
                Console.WriteLine(text);
            }
        }

        // Write Line (Overload)
        public static void WriteLine()
        {
            lock (__printLock)
            {
                resetCursor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
            }
        }

        // Write Two-Color Line
        public static void WriteTwoColorLine(ConsoleColor color1, string text1, ConsoleColor color2, string text2)
        {
            lock (__printLock)
            {
                resetCursor();
                Console.ForegroundColor = color1;
                Console.Write(text1);
                Console.ForegroundColor = color2;
                Console.WriteLine(text2);
            }
        }

        // Reset cursor position
        private static void resetCursor()
        {
            consoleTop = Console.GetCursorPosition().Top;
            Console.SetCursorPosition(consoleLeft, consoleTop);
        }
    }
}