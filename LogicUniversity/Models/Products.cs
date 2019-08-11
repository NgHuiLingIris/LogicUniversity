using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Products
    {
        

        [Key]
         public string ItemCode { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public double ReorderLevel { get; set; }
        public double ReorderQty { get; set; }
        public string UOM { get; set; }
        public double Balance { get; set; }
        public string Supplier1 { get; set; }
        public string Supplier2 { get; set; }
        public string Supplier3 { get; set; }
        public double UnitPrice { get; set; }
        public string SupplierId { get; set; }

        public List<StockAdjustmentVoucherDetail> StockAdjustmentVoucherDetails { get; set; }
        public virtual Supplier Supplier { get; set; }

    }
}