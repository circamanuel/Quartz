using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.Models
{
    internal class SessionModel
    {
        public Guid id {  get; set; }
        public  DateTime  StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public int FocusTimeInMinutes { get; set; }

    }
}
