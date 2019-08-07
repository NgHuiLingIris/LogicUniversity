using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    [Table("StockAdjustmentVoucherDetail")]
    public class StockAdjustmentVoucherDetail
    {
        [Key, Column(Order = 0)]
        public string StockAdjustmentVoucherId { get; set; }
        [Key, Column(Order = 1)]
        public string ItemCode { get; set; }

        public string Reason { get; set; }
        public double QuantityAdjusted { get; set; }
        public string Status { get; set; }
        public string ApproverRemarks { get; set; }
        public double Balance { get; set; }
        public String Approver { get; set; }

        [ForeignKey("ItemCode")]
        public Products Product { get; set; }

        [ForeignKey("StockAdjustmentVoucherId")]
        public StockAdjustmentVoucher StockAdjustmentVoucher { get; set; }
    }
}