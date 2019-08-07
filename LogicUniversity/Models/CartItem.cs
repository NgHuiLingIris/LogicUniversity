using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class CartItem
    {
        [Key]
        public string ItemCode { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int Quantity { get; set; }
        public int EmployeeId { get; set; }


    }
}