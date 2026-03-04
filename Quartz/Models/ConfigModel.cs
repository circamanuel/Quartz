using System;
using System.Collections.Generic;
using System.Text;
using Quartz.Enums;

namespace Quartz.Models
{
    internal class ConfigModel
    {
        public int FocusTime { get; set; }
        public int BreakTime { get; set; }  
        public int Cycles { get; set; } 
        public ProcessConstant UnlimitedCycles { get; set; }
        public bool HasFinished { get; set; }
    }
}
