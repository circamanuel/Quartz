using Quartz.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Services
{
    internal class PomodoroTimer
    {
        private PomodoroConfig _config;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private int _remainingTime;
        private int _remainingCycles;
        private int _focusTime;
        private int _breakTime;
        private int _breakFlag;

        public PomodoroTimer(PomodoroConfig config)
        {
            _config = config;
            _focusTime = config.FocusTime;
            _breakTime = config.BreakTime;  
            _remainingCycles = config.Cycles;
            Console.Clear();
            StartFocus();
        }

        // TODO: After Pause how do we restart the proccess?
        // where do we save the time past until now ?
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

        // TODO: Resume Logic
        public void Resume()
        {
            // TODO: we need to check somehow if it is pause maybe whit cancelation token if expired or not ?
            // OR if its restart then we use the saved time in _timeElapsed;
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
            _cts?.Cancel(); 
            Console.Clear();
            Console.WriteLine("Quartz rests. Your time was not wasted.");
        }

        // TODO: Skip logic
        // check wich flag is set end change it to the next phase
        // reset the timer to the time of the next phase
        // change cycle phase 
        // start phase "StartFocus()" or "StartBreak()"
        public void Skip()
        {
            Console.WriteLine("Skip function");

        }

        // TODO: config logic -> maybe whit a object?
        public void Config()
        {
            Console.WriteLine("config");
        }

        /* Gets the the amount of time for this phase and the name string.
         * Checks if time was paused.True if _remaining time is not 0
         * Decrements Cycle 
         * TimeSpan.FromSeconsds to convert secons to exact time 00:00
         */
        
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
                catch (Exception ex)
                {
                    throw new TaskCanceledException();
                }
            }

            DecideNextPhase();
        }

        private void DisplayStatus(String phase, TimeSpan time)
        {
            
            Console.SetCursorPosition(0, 0);    
            Console.WriteLine($"\r{phase}");
            Console.WriteLine($"\r{_remainingCycles} / {_config.Cycles}");
            Console.Write($"\r{time.ToString("mm\\:ss")}");

        }

        private void DecideNextPhase()
        {
            if (_remainingCycles > 1 && _breakFlag == 1 && _config.Cycles > 1)
            {
                StartBreak();

            } else if (_remainingCycles > 0 && _breakFlag == 0 && _config.Cycles > 1)
            {
                _remainingCycles--;
                StartFocus();

            } else
            {
                Console.Clear();
                Console.WriteLine("Done for today");
            }

        }

    }
}
