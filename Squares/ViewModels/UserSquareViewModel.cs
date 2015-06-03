using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{

    public class UserSquareViewModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public StopWatchModel StopWatch { get; set; }

        public bool IsHidden { get; set; }
    }
}