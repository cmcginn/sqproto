using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{

    public class Duration
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public long Milliseconds { get; set; }

        public static Duration FromMS(long ms)
        {
            var result = new Duration();
            result.Milliseconds = ms;
            var delta = (double)Math.Abs(result.Milliseconds) / 1000;
            var days = Math.Floor(delta / 86400);
            delta -= days * 86400;
            var hours = Math.Floor(delta / 3600) % 24;
            delta -= hours * 3600;
            var minutes = Math.Floor(delta / 60) % 60;
            delta -= minutes * 60;
            var seconds = delta % 60;

            result.Days = (int)days;
            result.Hours = (int)hours;
            result.Minutes = (int)minutes;
            result.Seconds = (int)seconds;
            return result;

        }

      

    }
}