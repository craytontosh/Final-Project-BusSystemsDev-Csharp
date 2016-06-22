using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MW100_T5_Reese
{
    public class Contribution
    {
        public int ContributionNo { get; set; }
        public string MemberID { get; set; }
        public int CheckNo { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string DesignatedFund { get; set; }
        public string Notes { get; set; }
        public DateTime ContributionDate { get; set; }
    }
}
