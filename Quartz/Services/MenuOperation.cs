using Quartz.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Timers;
using Quartz.Enums;
using System.Data;
using System.Media;

namespace Quartz.Services
{
    internal class MenuOperation
    {
        private EngineService _engine;
        private TimelineProcessor _timelineProzessor = new TimelineProcessor();
        private bool _quitFlagSet = false;
        public MenuOperation()
        {

        }

        public MenuOperation(EngineService engine)
        {
            _engine = engine;
        }

        /* Calls DisplayLogo() klass to show the logo
         * Shows selection Menu with tree options: Start, Stats or Exit
         * Calls HandleMenuChoice(choice): Choses which class to call from the menu choice
         */
        public async Task ShowMainMenu()
        {
            bool showSaved = false;

            while (true)
            {
                Console.Clear();
                DisplayLogo();

                if (showSaved)
                {
                    AnsiConsole.MarkupLine("[green]✓ Session saved![/]");
                    AnsiConsole.WriteLine();
                    showSaved = false;
                }

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Chose a [green] option[/]")
                        .AddChoices("Start", "Stats", "Exit"));

                await HandleMenuChoice(choice);

                if (_quitFlagSet)
                {
                    Console.Clear();
                    await SavingStatus();
                    _quitFlagSet = false;   
                    showSaved = true;      
                }
            }

        }
        
        /* Gets (string choice) and decides which class to call
         *  Start: 
         *      Gets Config data from user input
         *      Sets values in EngineService Constructor
         *      Starts the Timer
         *      awaits if there is a user input
         *  Stats:
         *       Calls timeline stats Class
         *  Exit:
         *       Quits appliacation
         */
        private async Task HandleMenuChoice(string choice)
        {
            switch (choice)
            {
                case "Start":
                    var config = GetConfigFromUser();
                    _engine.SetConfig(config);

                    _ = _engine.StartFocus();           // fire-and-forget


                    await HandleInputDuringTimer();      // await den Input-Loop

                    break;

                case "Stats":
                    Console.Clear();
                    await _timelineProzessor.TimelineJsonRader();
                    break;

                case "Exit":
                    _engine.Exit();
                    return;
            }
        }

        private async Task HandleInputDuringTimer()
        {
            while (!_engine.HasFinished)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    await HandleInputAsync(key.Key);
                }

                await Task.Delay(50);
            }

            _quitFlagSet = true; 
        }

        public async Task HandleInputAsync(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Spacebar:
                    _ = _engine.ResumeAsync();  // Pause/Resume
                    break;

                case ConsoleKey.Escape:
                    _engine.Resume(ProcessConstant.QuitSessionFlag);
                    await Task.Delay(200);
                    QuitSessionPanel();
                    string choice = QuitPanelSelection(); 
                    if (choice == "Cancel")
                    {
                        _ = _engine.ResumeAsync();
                        break;
                    }

                    _engine.Quit();
                    break;

                case ConsoleKey.C:
                    break;
            }
        }
        private ConfigModel GetConfigFromUser()
        {
            
            var config = new ConfigModel();

            // Set focus time in minutes
            // Set break time in minutes
            AnsiConsole.MarkupLine($"Enter [Blue]Focus[/] time in Minutes: ");

            try
            {
                config.FocusTime = int.Parse(Console.ReadLine());
            } 
            catch
            {
                config.FocusTime = ErrorCall();
            }

            // Set break time in minutes
            AnsiConsole.MarkupLine($"Enter Break [blue]time[/] in Minutes: ");

            try
            {
                config.BreakTime = int.Parse(Console.ReadLine());
            }
            catch             
            {
                config.BreakTime = ErrorCall();
            }

            AnsiConsole.MarkupLine($"Enter amount of [blue]Cycles[/], to skip enter: ");
            string stringCycle = Console.ReadLine();    

            if(string.IsNullOrEmpty(stringCycle))
            {
                config.UnlimitedCycles = ProcessConstant.Unlimited; 
                Console.WriteLine("skiped cycles");
            }
            else
            {
                try
                {
                    int cycles = int.Parse(stringCycle);
                    config.Cycles = cycles; 
                }
                catch
                {
                    ErrorCall(); 
                }
            }

            Console.WriteLine("Quartz: Youre Timer Started !!");

            return config;  
        }

        private int ErrorCall(string promptString = "Please Enter a number: ")
        {
            //return int.Parse(Console.ReadLine());

            while (true)
            {
                try
                {
                    var input = AnsiConsole.Prompt<string>(
                        new TextPrompt<string>($"[yellow]{promptString}[/]")
                        { AllowEmpty = false });

                    if (!int.TryParse(input, out int value))
                    {
                        AnsiConsole.MarkupLine($"[red]x Only numbers![/]");
                        continue;
                    }

                    if(value <= 0)
                    {
                        AnsiConsole.MarkupLine($"[red]x Only Positive numbers![/]");
                    }

                    return value;

                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]x Error: {ex.Message}[/]");
                }
            }

        }

        private void DisplayLogo()
        {

            Console.WriteLine(@"                                                         
      ___           ___           ___           ___                         ___     
     /  /\         /  /\         /  /\         /  /\          ___          /__/\    
    /  /::\       /  /:/        /  /::\       /  /::\        /__/\         \  \:\   
   /__/:/\:\     /  /:/        /  /:/\:\     /  /:/\:\       \  \:\         \  \:\  
   \  \:\ \:\   /  /:/        /  /::\ \:\   /  /::\ \:\       \__\:\         \  \:\ 
    \  \:\ \:\ /__/:/     /\ /__/:/\:\_\:\ /__/:/\:\_\:\      /  /::\   ______\__\:\
     \  \:\/:/ \  \:\    /:/ \__\/  \:\/:/ \__\/~|::\/:/     /  /:/\:\ \  \::::::::/
      \__\::/   \  \:\  /:/       \__\::/     |  |:|::/     /  /:/__\/  \  \:\~~~~~ 
      /  /:/     \  \:\/:/        /  /:/      |  |:|\/     /__/:/        \  \:\     
     /__/:/       \  \::/        /__/:/       |__|:|~      \__\/          \  \:\    
     \__\/         \__\/         \__\/         \__\|                       \__\/    
            ");

            Console.WriteLine("Hei there , Welcome to Quartz! ");
            // Styled text with markup
            AnsiConsole.MarkupLine("[bold blue]Welcome[/] to [green]Spectre.Console[/]!");
        }

        private void QuitSessionPanel()
        {
            //Console.SetCursorPosition(0, 0);    
            var panel = new Spectre.Console.Panel("[bold]Do you realy want to quit this session ?[/]")
                .Header("[yellow]Warning[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Spectre.Console.Color.DarkRed)
                .Padding(2, 1)
                .Expand();

            AnsiConsole.Write(panel);

        }

        private string QuitPanelSelection()
        {
            //Console.SetCursorPosition(10, 0);    
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Cancel", "End Session"));

            return choice;

        }

        private async Task QuitPanelExcecuterAsync(string choice)
        {
            if (choice == "Cancel")
            {
                _ = _engine.ResumeAsync();
                return;
            }
            else
            {
                _engine.Quit();
            }

        }

        public async Task SavingStatus()
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("yellow"))
                .StartAsync("Saving session...", async ctx =>
                {
                    await Task.Delay(2000);
                });

            AnsiConsole.MarkupLine("[green]✓ Session saved![/]");
        }
    }
}
