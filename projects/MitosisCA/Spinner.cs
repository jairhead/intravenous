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

        // Stop animation (good)
        public void stopGood()
        {
            if (active)
            {
                active = false;
                ConsoleLogger.WriteLine(ConsoleColor.Green, "(^_^)");
            }
        }

        // Stop animation (interrupt)
        public void stopInterrupt()
        {
            if (active)
            {
                active = false;
                ConsoleLogger.WriteLine(ConsoleColor.Yellow, "(-_-)");
            }
        }

        // Stop animation (bad)
        public void stopBad()
        {
            if (active)
            {
                active = false;
                ConsoleLogger.WriteLine(ConsoleColor.Red, "(>_<)");
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

        // Turn the spinner
        private void turn()
        {
            ConsoleLogger.Write(ConsoleColor.Cyan, $"[{sequence[++counter % sequence.Length]}]");
        }
    }
}
