using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class DisbursementDetail
    {
        [Key, Column(Order = 0)]
        public int DisbursementId { get; set; }
        [Key, Column(Order = 1)]
        public string ItemCode { get; set; }

        public int QuantityRequested { get; set; }
        public int QuantityReceived { get; set; }
        public string AdjustmentVoucherId { get; set; }

        [ForeignKey("AdjustmentVoucherId")]
        public StockAdjustmentVoucher StockAdjustmentVoucher { get; set; }

        [ForeignKey("DisbursementId")]
        public Disbursement Disbursement { get; set; }

        [ForeignKey("ItemCode")]
        public Products Products { get; set; }
    }
}
