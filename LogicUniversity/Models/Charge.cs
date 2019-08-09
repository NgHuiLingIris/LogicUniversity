using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Charge
    {
        public DisbursementDetail disburementDetail { get; set; }
        public Products product { get; set; }
    }
}