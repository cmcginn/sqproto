using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public class UserSquaresViewModel
    {
        private List<UserSquareViewModel> _UserSquares;
        public string UserId { get; set; }
        public List<UserSquareViewModel> UserSquares
        {
            get { return _UserSquares; }
            set { _UserSquares = value; }
        }
    }
}