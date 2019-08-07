using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    [Table("StockCard")]
    public class StockCard
    {
        [Key, Column(Order = 0)]
        public string ItemCode { get; set; }
        [Key, Column(Order = 1)]
        public int BinNo { get; set; }

        [ForeignKey("ItemCode")]
        public Products product { get; set; }
    }
}