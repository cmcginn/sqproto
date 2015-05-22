using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public class UserSquareActionModel
    {
        public bool Start { get; set; }
        public Guid Id { get; set; }
        public long Elapsed { get; set; }

        public string UserId { get; set; }
    }
}