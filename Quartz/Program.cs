using Quartz.Models;
using Quartz.Services;

namespace Quartz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new PomodoroConfig();
            CancellationTokenSource source = new CancellationTokenSource();

            Console.CursorVisible = false;
            Console.WriteLine("                                     \r\n                 ▒░░▓                \r\n                ▒░ ░▒▒               \r\n               ▒▒░  ░░▒              \r\n               ▒░    ░░░             \r\n              ░░        ░            \r\n             ░░░        ░░           \r\n            ░░░░░░      ░            \r\n            ░  ░░░      ░░           \r\n                ░░       ░░░         \r\n                ░░░      ░░░         \r\n               ░░░░░░░░░░░░░         \r\n              ░░░░░░░░░░░░░░░        \r\n              ░░░░░░░░▒▒▒▒░░░        \r\n                ░░▒▒▒▒▒▒▒░░          \r\n         ░░   ░░░░░░░▒▒▒▒▒           \r\n            ░░░░░░░░░░▒▓▒░           \r\n            ░░▒▒▓▒▒▒▒▒▓▓             \r\n                 ▒▒▒▒                \r\n                                     ");
            Console.WriteLine("Hei there , Welcome to Quartz! ");
            // +"To start press enter to exit backspace");

            // Set focus time in minutes
            Console.WriteLine("Enter Focus time in Minutes");
            config.FocusTime = int.Parse(Console.ReadLine());
            Console.WriteLine($"Focus Time set to: {config.FocusTime} Minutes");

            // Set break time in minutes
            Console.WriteLine("Enter Break time in Minutes");
            config.BreakTime = int.Parse(Console.ReadLine());
            Console.WriteLine($"Breaktime set to: {config.BreakTime} Minutes");

            // amount of cycles
            Console.WriteLine("Enter amount of Cycles, to skip enter");

            string input = Console.ReadLine();

            if (int.TryParse(input, out int cycles))
            {
                config.Cycles = cycles; 
                Console.WriteLine($"Cycles: {config.Cycles}");
            }
            else
            {
                Console.WriteLine("skiped cycles");
            }

            Console.WriteLine("Quartz: Youre Timer Started !!");

            // Settings switch
            Console.WriteLine(@"Settings: 
                Space to Pause/Resume 
                Q to Quit
                S to Skip Cycle+
                C to Config times and Cycle");

            // Start timer
            var timer = new PomodoroTimer(config);
            bool stop = false;

            //TODO: Somehow after resume we allways go in to resume and spacebar is allways set. need to reset this to go back to the timer.
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

                        case ConsoleKey.S:
                            timer.Skip();
                            Console.WriteLine("S");
                            break;

                        case ConsoleKey.C:
                            timer.Config();
                            Console.WriteLine("");
                            break;
                    }
                }
            }
        }
    }
}
