using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.Models
{
    internal class PomodoroSessions
    {
        //public int CycleCount { get; set; }
        public  DateTime  StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public int FocusTimeInMinutes { get; set; }

    }
}
