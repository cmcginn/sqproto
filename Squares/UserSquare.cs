//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Squares
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserSquare
    {
        public UserSquare()
        {
            this.StopWatches = new HashSet<StopWatch>();
        }
    
        public System.Guid Id { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }
        public System.DateTime CreratedOnUtc { get; set; }
        public bool Hidden { get; set; }
    
        public virtual ICollection<StopWatch> StopWatches { get; set; }
    }
}
