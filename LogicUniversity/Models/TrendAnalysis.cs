using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class TrendAnalysis
    {
        
        
        public int OrderEmployeeId { get; set; }
        public DateTime DateOrdered { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public string DeptName { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        //List<TrendAnalysis> TrendAnalysiss{ get; set; }

    }
}