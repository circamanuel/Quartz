using Quartz.Models;
using Quartz.Services;

namespace Quartz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO: Boot up menu whit multiple options selected by arrow key.
            // Selection will be highlightet
            // Options: Start, Timeline and exit

            // Start timer
            var timer = new PomodoroTimer();

            bool stop = false;

            while (!stop)
            {
                // checks if key is entered
                if (Console.KeyAvailable)
                {

                    // Hide key in terminal
                    var key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            timer.Resume();
                            break;

                        case ConsoleKey.Q:
                            stop = true;
                            timer.Quit();
                            break;
                    }
                }
            }
        }
    }
}
