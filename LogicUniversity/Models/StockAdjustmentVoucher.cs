using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    [Table("StockAdjustmentVoucher")]
    public class StockAdjustmentVoucher
    {
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }

        public List<StockAdjustmentVoucherDetail> StockAdjustmentVoucherDetails { get; set; }
    }
}