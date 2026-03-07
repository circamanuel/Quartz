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
using System.Media;

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
        private NotificationService notificationService;
        public bool HasFinished { get; private set; }

        private int _totalTime; 
        public EngineService()
        {

            Console.CursorVisible = false;
            notificationService = new NotificationService();
        }

        public void SetConfig(ConfigModel config)
        {
            _config = config;
            _focusTime = config.FocusTime;
            _breakTime = config.BreakTime;
            _remainingCycles = config.Cycles;
            _startDate = DateTime.Now;
            _remainingTime = 0;
            _totalTime = 0; // ← hinzufügen
            _alreadySaved = ProcessConstant.NotSaved;  // <-- reset
            _cts = new CancellationTokenSource();       // <-- frische CTS
            HasFinished = false;                        // <-- richtig
        }

        /*
         * Starts Timer and sets break flag
         */
        public async Task StartFocus()
        {
            Console.Clear();
            String focusPhase = "Focus time ";
            _breakFlag = ProcessConstant.Break;
            await RunCountDown(_focusTime, focusPhase);
        }
       
        /*
         * Starts break timer and set flag to 0
         */
        public async Task StartBreak()
        {
            Console.Clear();
            string breakPhase = "Break time ";
            _breakFlag = 0;
            await RunCountDown(_breakTime, breakPhase);

        }

        /*
         * Resums timer if token is cancelled is requested
         * Restarts timer if there is no token cancelation request
         * Sets fresh cancellation token 
         */
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
                await Task.Delay(100);
                Console.Clear();
                var panel = new Spectre.Console.Panel("Resting...")
                    .RoundedBorder()
                    .BorderColor(Spectre.Console.Color.Orange1);

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
        
        /*
         * Quits Session and Fills data to Session model
         * Calls Append function to covert session model to JSON
         * Cancels cancellation token
         * Auits notification service
         * Set flag hasFinished = true
         */
        public void Quit()
        {
            if (_alreadySaved == ProcessConstant.Saved) return;
            _alreadySaved = ProcessConstant.Saved;

            _cts?.Cancel();
            notificationService.Dispose();
            System.Threading.Thread.Sleep(200);

            var session = new SessionModel
            {
                id = Guid.NewGuid(),
                StartDate = _startDate,
                EndDate = DateTime.Now,
                FocusTimeInMinutes = _focusTime
            };

            _session.Append(session);
            HasFinished = true;

            Console.Clear();
        }

         /*
          * Quits Application
          */
        public void Exit()
        {
            Quit();  // Erst aufräumen
            System.Threading.Thread.Sleep(300);  // Warte
            Environment.Exit(0);  // DANN beenden
        }

       /* Runs Countdown timer with Spectre.Console Loading bar 
        * Calls Next phase function
        */
        private async Task RunCountDown(int time, string phase)
        {
            if (_remainingTime == 0)
            {
                _remainingTime = time * 60;
                _totalTime = _remainingTime;
            }

            string status = DisplayStatus(phase);

            try
            {
                await AnsiConsole.Progress()
                    .Columns(
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn())
                    .StartAsync(async ctx =>
                    {
                        int alreadyElapsed = _totalTime - _remainingTime;
                        var task = ctx.AddTask($"[blue]{status}[/]", maxValue: _totalTime);
                        task.Value = alreadyElapsed;

                        for (int i = _remainingTime; i >= 0; i--)
                        {
                            _remainingTime = i;
                            int minutes = _remainingTime / 60;
                            int seconds = _remainingTime % 60;

                            task.Description = $"[blue]{status}[/] [yellow]{minutes:D2}:{seconds:D2}[/]";
                            task.Value = _totalTime - _remainingTime;

                            await Task.Delay(1000, _cts.Token);
                        }
                    });
            }
            catch (TaskCanceledException)
            {
                return;
            }

            _remainingTime = 0;
            _totalTime = 0;
            await DecideNextPhaseAndIsInfinitCyclesAsync();
        }

        // Returns Cycle status
        private string DisplayStatus(String phase)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);    
            Console.WriteLine($"\r{phase}");

            if (_config.UnlimitedCycles != ProcessConstant.Unlimited) 
            {
                return $"\r{_remainingCycles} / {_config.Cycles}";
            }
            else
            {
                //return "\u221E";
                return "Infinit";
                // infinty ascii does't work..
            }
        }
         
        private async Task DecideNextPhaseAndIsInfinitCyclesAsync()
        {

            if (
                _remainingCycles > 1 && _breakFlag == ProcessConstant.Break && _config.Cycles > 1 ||
                _config.UnlimitedCycles == ProcessConstant.Unlimited && _breakFlag == ProcessConstant.Break
                )
            {
                notificationService.ShowPauseNotification();
                await StartBreak();

            } else if (
                _remainingCycles > 0 && _breakFlag == 0 && _config.Cycles > 1 ||
                _config.UnlimitedCycles == ProcessConstant.Unlimited && _breakFlag == 0 
                )
            {
                _remainingCycles--;
                notificationService.ShowTimerCompleteNotification();
                await StartFocus();

            } else
            {
                notificationService.ShowAllCompleteNotification();
                Console.Clear();
                Quit();
            }

        }
    }
}
