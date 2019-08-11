using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class OrderDetail
    {
        [Key, Column(Order = 0)]
        public int PONumber { get; set; }
        [Key, Column(Order = 1)]
        public string ItemCode { get; set; }
        public double Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Products Products { get; set; }
    }
}