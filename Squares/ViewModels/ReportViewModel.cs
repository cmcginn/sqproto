using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public class ReportViewModel
    {
        private List<ReportItemViewModel> _ReportItems;

        public List<ReportItemViewModel> ReportItems
        {
            get { return _ReportItems??(_ReportItems=new List<ReportItemViewModel>()); }
            set { _ReportItems = value; }
        }
    }
}