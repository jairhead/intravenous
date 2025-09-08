// Spinner.cs
// Contains the Spinner class

using System.ComponentModel;
using FileUtils;

namespace TerminalUtils
{
    public class Spinner
    {
        // Members
        private const string sequence = @"/-\|";
        private int counter = 0;
        private readonly int delay;
        private bool active;
        private readonly Thread thread;

        // Constructor
        public Spinner(int delay = 100)
        {
            left = Console.GetCursorPosition().Left;
            top = Console.GetCursorPosition().Top;
            this.delay = delay;
            thread = new Thread(spin);
        }

        // Start animation
        public void start()
        {
            active = true;
            if (!thread.IsAlive)
            {
                thread.Start();
            }
        }

        // Stop animation
        public void stop()
        {
            if (active)
            {
                active = false;
                draw('X', ConsoleColor.Red);
                ConsoleLogger.WriteLine();
            }
        }

        // Spin
        private void spin()
        {
            while (active)
            {
                turn();
                Thread.Sleep(delay);
            }
        }

        // Draw a character
        private void draw(char c, ConsoleColor color)
        {
            ConsoleLogger.Write(color, $"[{c}]");
        }

        // Turn the spinner
        private void turn()
        {
            draw(sequence[++counter % sequence.Length], ConsoleColor.Cyan);
        }
    }
}