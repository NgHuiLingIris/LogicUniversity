using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class TempRetrieval
    {
        public int RequisitionId { get; set; }
        public int RequisitionDetailId { get; set; }
        public string StationeryDescription { get; set; }
        public double QuantityInInventory { get; set; }
        public string Itemcode { get; set; }
        public double QuantityRequested { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public DateTime ApprovalDate { get; set; }

    }
}