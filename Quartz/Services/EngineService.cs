using Quartz.Models;
using Quartz.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Quartz.Persistence;
using Quartz.Enums;
using System.Reflection.Metadata;
using Spectre.Console;
using System.Data;

namespace Quartz.Services
{
    internal class EngineService
    {
        private ConfigModel _config;
        private TimelineLogRepository _session = new TimelineLogRepository();    
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private int _remainingTime;
        private int _remainingCycles;
        private int _focusTime;
        private int _breakTime;
        private ProcessConstant _breakFlag;
        private DateTime _startDate;
        private ProcessConstant _alreadySaved = ProcessConstant.NotSaved;
        public bool HasFinished { get; private set; }

        public EngineService()
        {

            //Console.Clear();
            Console.CursorVisible = false;
        }

        public void SetConfig(ConfigModel config)
        {
            _config = config;
            _focusTime = config.FocusTime;
            _breakTime = config.BreakTime;
            _remainingCycles = config.Cycles;
            _startDate = DateTime.Now;
            _remainingTime = 0;
            _alreadySaved = ProcessConstant.NotSaved;  // <-- reset
            _cts = new CancellationTokenSource();       // <-- frische CTS
            HasFinished = false;                        // <-- richtig
        }

        public async Task StartFocus()
        {
            Console.Clear();
            String focusPhase = "Focus time ";
            _breakFlag = ProcessConstant.Break;
            await RunCountDown(_focusTime, focusPhase);
        }
        
        public async Task StartBreak()
        {
            string breakPhase = "Break time ";
            _breakFlag = 0;
            await RunCountDown(_breakTime, breakPhase);

        }

        public async Task ResumeAsync()
        {
            // If there is no Token = True
            if (_cts.Token.IsCancellationRequested)
            {

                _cts = new CancellationTokenSource();   

                switch (_breakFlag)
                {
                    case 0: 
                        await StartBreak();
                        break;

                    default:
                        await StartFocus();
                        break;
                }

            } else
            {
                // If there is a token we gone cancel it here
                _cts.Cancel();
                Console.WriteLine("");
                var panel = new Panel("Resting...")
                    .RoundedBorder()
                    .BorderColor(Color.Orange1);

                AnsiConsole.Write(panel);
            }
        }
        public void Resume(ProcessConstant flag)
        {
            if (_cts.Token.IsCancellationRequested && flag != ProcessConstant.QuitSessionFlag)
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
                if (flag == ProcessConstant.QuitSessionFlag && _cts.Token.IsCancellationRequested)
                {
                    return;
                }
                _cts.Cancel();
            }
        }



        public void Quit()
        {
            if (_alreadySaved == ProcessConstant.Saved) return;
            _alreadySaved = ProcessConstant.Saved;

            _cts?.Cancel();
            System.Threading.Thread.Sleep(200);

            var session = new SessionModel
            {
                id =Guid.NewGuid(),
                StartDate = _startDate,
                EndDate = DateTime.Now,
                FocusTimeInMinutes = _focusTime
            };

            _session.Append(session);
            HasFinished = true;
            
            Console.Clear();
            Console.WriteLine("Quartz rests. Your time was not wasted.");

            //Environment.Exit(0);
        }
        public void Exit()
        {
            Quit();  // Erst aufräumen
            System.Threading.Thread.Sleep(300);  // Warte
            Environment.Exit(0);  // DANN beenden
        }

        
        private async Task RunCountDown(int time, String phase )
        {
            if (_remainingTime == 0)
            {
                _remainingTime = time * 60;
            }

            for (int i = _remainingTime; i >= 0; i--)
            {
                _remainingTime = i;

                TimeSpan preciseDuraiont = TimeSpan.FromSeconds(_remainingTime);
                DisplayStatus(phase, preciseDuraiont);

                try
                {
                    await Task.Delay(1000, _cts.Token);
                    //await Task.Delay(100, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }

            await DecideNextPhaseAndIsInfinitCyclesAsync();
        }

        private void DisplayStatus(String phase, TimeSpan time)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);    
            Console.WriteLine($"\r{phase}");

            if (_config.UnlimitedCycles != ProcessConstant.Unlimited) 
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

        private async Task DecideNextPhaseAndIsInfinitCyclesAsync()
        {
            if (
                _remainingCycles > 1 && _breakFlag == ProcessConstant.Break && _config.Cycles > 1 ||
                _config.UnlimitedCycles == ProcessConstant.Unlimited && _breakFlag == ProcessConstant.Break
                )
            {
                await StartBreak();

            } else if (
                _remainingCycles > 0 && _breakFlag == 0 && _config.Cycles > 1 ||
                _config.UnlimitedCycles == ProcessConstant.Unlimited && _breakFlag == 0 
                )
            {
                _remainingCycles--;
                await StartFocus();

            } else
            {
                Console.Clear();
                Quit();
            }

        }
    }
}
