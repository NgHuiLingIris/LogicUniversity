using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Order
    {
        [Key]
        //to prevent auto-generation of PK, use: [DatabaseGenerated(DatabaseGeneratedOption)]
        public int POnumber { get; set; }
        public int DOnumber { get; set; }
        public string SupplierCode { get; set; }
        public int OrderEmployeeId { get; set; }
        
        public DateTime DateOrdered { get; set; }
        public DateTime DateDelivery { get; set; }
        public string Status { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}