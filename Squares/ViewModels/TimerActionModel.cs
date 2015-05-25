using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public class TimerActionModel
    {
        public long Elapsed { get; set; }
        public long Time { get; set; }
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public ActivityStateTypes ActivityState { get; set; }
    }
}