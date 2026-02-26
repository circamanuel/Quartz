using Quartz.Models;
using Quartz.Services;

namespace Quartz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new PomodoroConfig();  


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

            while (true)
            {
                // checks if key is entered
                if (Console.KeyAvailable)
                {
                    // PomodoroTimer object
                    var timer = new PomodoroTimer(config);
                    // Hide key in terminal
                    var key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            //timer.Resume();
                            Console.WriteLine("spacebar");
                            break;

                        case ConsoleKey.Q:
                            timer.Quit();
                            Console.WriteLine("Q");
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
