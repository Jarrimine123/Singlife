using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Singlife.Model
{
    public class ReadinessProfile
    {
        public int ProfileID { get; set; }
        public int AccountID { get; set; }
        public int Q1Answer { get; set; }
        public int Q2Answer { get; set; }
        public int Q3Answer { get; set; }
        public int Q4Answer { get; set; }
        public int Q5Answer { get; set; }
        public int TotalScore { get; set; }
        public int ReadinessLevel { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}