﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{

    public class UserSquareViewModel
    {
        public string Name { get; set; }
        public long Elapsed { get; set; }
        public long Milliseconds { get; set; }
        public Guid Id { get; set; }
        public Guid UserSquareActivityId { get; set; }
        public ActivityStateTypes ActivityState { get; set; }
        public long RunningTime { get; set; }
        public bool Visible { get; set; }
        public long Started { get; set; }
    }
}