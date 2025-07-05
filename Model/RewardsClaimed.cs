using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Singlife.Model
{
    public class RewardsClaimed
    {
        public int RewardID { get; set; }
        public int AccountID { get; set; }
        public int VoucherTypeID { get; set; }
        public int AmountClaim { get; set; }
        public DateTime DateCreated { get; set; }
    }
}