using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Singlife.Model
{
    public class JourneyMapping
    {
        public int MilestoneID { get; set; }
        public int AccountID { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}