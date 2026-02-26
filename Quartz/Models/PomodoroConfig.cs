using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.Models
{
    internal class PomodoroConfig
    {
        public int FocusTime { get; set; }
        public int BreakTime { get; set; }  
        public int Cycles { get; set; } 
    }
}
