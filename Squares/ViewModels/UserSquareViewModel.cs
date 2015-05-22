using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public enum ActivityStateTypes
    {
        None=0,
        Started=1,
        Paused=2,
        Stopped=3
    }
    public class UserSquareViewModel
    {
        public string Name { get; set; }
        public long Elapsed { get; set; }
        public Guid Id { get; set; }
        public ActivityStateTypes ActivityState { get; set; }
        public long? StartDate { get; set; }


    }
}