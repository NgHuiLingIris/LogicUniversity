using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class RetrievalDetail
    {
        [Key, Column(Order = 0)]
        public int RetrievalId { get; set; }
        [Key, Column(Order = 1)]
        public string ItemCode { get; set; }
        public int QuantityRetrieved { get; set; }
        public int QuantityNeeded { get; set; }
        public string AdjustmentVoucherId { get; set; }

        [ForeignKey("AdjustmentVoucherId")]
        public StockAdjustmentVoucher StockAdjustmentVoucher { get; set; }

        [ForeignKey("RetrievalId")]
        public Retrieval Retrieval { get; set; }

        [ForeignKey("ItemCode")]
        public Products Products { get; set; }
    }
}
