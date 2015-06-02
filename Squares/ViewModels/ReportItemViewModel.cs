using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Squares.ViewModels
{
    public class ActivityRecord
    {
        public Guid Id { get; set; }
        public long StartDate { get; set; }
        public long Elapsed { get; set; }
        public long Started { get; set; }
        public long Ended { get; set; }
       
    }
    public class ReportItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public long TotalDuration { get; set; }

        public long Started { get; set; }

        public ActivityStateTypes State { get; set; }
        public bool IsDeleted { get; set; }
        public List<ActivityRecord> ActivityRecords
        {
            get { return _ActivityRecords ?? (_ActivityRecords = new List<ActivityRecord>()); }
            set { _ActivityRecords = value; }
        }

        private List<ActivityRecord> _ActivityRecords;

    }
}