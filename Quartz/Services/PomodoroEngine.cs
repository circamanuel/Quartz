using Quartz.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Quartz.Persistence;

namespace Quartz.Services
{
    internal class PomodoroEngine
    {
        private PomodoroConfig _config;
        private TimelineLogRepository _session = new TimelineLogRepository();    
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private int _remainingTime;
        private int _remainingCycles;
        private int _focusTime;
        private int _breakTime;
        private int _breakFlag;
        private bool _unlimitedCycles;
        private DateTime _startDate;
        private bool _alreadySaved = false;

        public PomodoroEngine()
        {
            PomodoroConfig returnedConfig = BootUpConfig();

            _config = returnedConfig;
            _focusTime = returnedConfig.FocusTime;
            _breakTime = returnedConfig.BreakTime;
            _remainingCycles = returnedConfig.Cycles;
            _startDate = DateTime.Now;


            Console.Clear();

            Console.CursorVisible = false;

            StartFocus();
        }

        private PomodoroConfig BootUpConfig()
        {

            var config = new PomodoroConfig();

            Console.WriteLine("                                     \r\n                 ▒░░▓                \r\n                ▒░ ░▒▒               \r\n               ▒▒░  ░░▒              \r\n               ▒░    ░░░             \r\n              ░░        ░            \r\n             ░░░        ░░           \r\n            ░░░░░░      ░            \r\n            ░  ░░░      ░░           \r\n                ░░       ░░░         \r\n                ░░░      ░░░         \r\n               ░░░░░░░░░░░░░         \r\n              ░░░░░░░░░░░░░░░        \r\n              ░░░░░░░░▒▒▒▒░░░        \r\n                ░░▒▒▒▒▒▒▒░░          \r\n         ░░   ░░░░░░░▒▒▒▒▒           \r\n            ░░░░░░░░░░▒▓▒░           \r\n            ░░▒▒▓▒▒▒▒▒▓▓             \r\n                 ▒▒▒▒                \r\n                                     ");
            Console.WriteLine("Hei there , Welcome to Quartz! ");

            Console.WriteLine(@"
            Settings: 
            Space to Pause/Resume 
            Q to Quit
            C to Config times and Cycle
            ");
           

            // Set focus time in minutes
            Console.Write("Enter Focus time in Minutes: ");
            config.FocusTime = int.Parse(Console.ReadLine());
            Console.WriteLine($"\rFocus Time set to: {config.FocusTime} Minutes");

            // Set break time in minutes
            Console.Write("Enter Break time in Minutes: ");
            config.BreakTime = int.Parse(Console.ReadLine());
            Console.WriteLine($"Breaktime set to: {config.BreakTime} Minutes");

            // amount of cycles
            Console.Write("Enter amount of Cycles, to skip enter: ");

            string input = Console.ReadLine();

            if (int.TryParse(input, out int cycles))
            {
                config.Cycles = cycles; 
                Console.WriteLine($"Cycles: {config.Cycles}");
            }
            else
            {
                _unlimitedCycles = true;
                Console.WriteLine("skiped cycles");
            }

            Console.WriteLine("Quartz: Youre Timer Started !!");

            return config;  
        }

        public async Task StartFocus()
        {
            String focusPhase = "Focus time ";
            _breakFlag = 1;
            await RunCountDown(_focusTime, focusPhase);
        }
        
        public async Task StartBreak()
        {
            string breakPhase = "Break time ";
            _breakFlag = 0;
            await RunCountDown(_breakTime, breakPhase);

        }

        public void RunCycles()
        {
            int cycles = _config.Cycles;
            Console.WriteLine($"{cycles} Cycles set");
        }

        public void Resume()
        {
            if (_cts.Token.IsCancellationRequested)
            {

                _cts = new CancellationTokenSource();   

                switch (_breakFlag)
                {
                    case 0: 
                        StartBreak();
                        break;

                    default:
                        StartFocus();
                        break;
                }

            } else
            {
                _cts.Cancel();
            }
        }

        // TODO: Quit logic
        // 
        public void Quit()
        {
            if (_alreadySaved) return;
            _alreadySaved = true;

            _cts?.Cancel(); 

            var session = new PomodoroSession
            {
                id =Guid.NewGuid(),
                StartDate = _startDate,
                EndDate = DateTime.Now,
                FocusTimeInMinutes = _focusTime
            };

            _session.Append(session);
            
            Console.Clear();
            Console.WriteLine("Quartz rests. Your time was not wasted.");

            Environment.Exit(0);

        }

        public void Skip()
        {
            Console.WriteLine("Skip function");

        }
        
        private async Task RunCountDown(int time, String phase )
        {
            if (_remainingTime == 0)
            {
                _remainingTime = time * 60;
            }

            for (int i = _remainingTime; i >= 0; i--)
            {
                //TODO: Was passiert bei pausen
                _remainingTime = i;

                TimeSpan preciseDuraiont = TimeSpan.FromSeconds(_remainingTime);
                DisplayStatus(phase, preciseDuraiont);

                try
                {
                    //await Task.Delay(1000, _cts.Token);
                    await Task.Delay(100, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }

            DecideNextPhaseAndIsInfinitCycles();
        }

        private void DisplayStatus(String phase, TimeSpan time)
        {
            
            Console.SetCursorPosition(0, 0);    
            Console.WriteLine($"\r{phase}");

            if (!_unlimitedCycles) 
            {
                Console.WriteLine($"\r{_remainingCycles} / {_config.Cycles}");
            }
            else
            {
                Console.WriteLine("Infinty");
                // infinty ascii does't work..
                //Console.WriteLine("\u221E");
            }

            Console.Write($"\r{time.ToString("mm\\:ss")}");

        }

        private void DecideNextPhaseAndIsInfinitCycles()
        {
            if (
                _remainingCycles > 1 && _breakFlag == 1 && _config.Cycles > 1 ||
                _unlimitedCycles == true && _breakFlag == 1
                )
            {
                StartBreak();

            } else if (
                _remainingCycles > 0 && _breakFlag == 0 && _config.Cycles > 1 ||
                _unlimitedCycles == true && _breakFlag == 0 
                )
            {
                _remainingCycles--;
                StartFocus();

            } else
            {
                Console.Clear();
                Quit();
            }

        }
    }
}
