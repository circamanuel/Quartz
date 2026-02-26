using Quartz.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.Services
{
    internal class PomodoroTimer
    {
        private PomodoroConfig _config;

        public PomodoroTimer(PomodoroConfig config)
        {
            _config = config;
            StartFocus();
        }

        public void StartFocus()
        {
            int focusTime = _config.FocusTime;
            Console.WriteLine($"focus time set to {focusTime}");
        }

        public void StartBreak()
        {
            int breakTime = _config.BreakTime;      
            Console.WriteLine($"{breakTime} Min break");
        }

        public void RunCycles()
        {
            int cycles = _config.Cycles;
            Console.WriteLine($"{cycles} Cycles set");
        }

        // TODO: Resume Logic
        public void Resume()
        {
            Console.WriteLine("Resume funktion");
        }

        // TODO: Quit logic
        public void Quit()
        {
           
            Console.WriteLine("Quit function");
        }

        // TODO: Skip logic
        public void Skip()
        {
            Console.WriteLine("Skip function");

        }

        // TODO: config logic -> maybe whit a object?
        public void Config()
        {
            Console.WriteLine("config");
        }

    }
}/**/
