using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public class StopWatchModel
    {
        public Guid Id { get; set; }
        public long Time { get; set; }
        public long Started { get; set; }
        public ActivityStateTypes State { get; set; }
    }
}