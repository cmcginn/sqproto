using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public class ActivityRecord
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public long Duration { get; set; }

        public string ActivityState { get; set; }
    }
    public class ReportItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public long TotalDuration { get; set; }

        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public List<ActivityRecord> ActivityRecords
        {
            get { return _ActivityRecords ?? (_ActivityRecords = new List<ActivityRecord>()); }
            set { _ActivityRecords = value; }
        }

        private List<ActivityRecord> _ActivityRecords;

    }
}