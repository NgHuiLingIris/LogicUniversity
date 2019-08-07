using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    [Table("Supplier")]
    public class Supplier
    {
        [Key]
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public int Phone { get; set; }
        public int Fax { get; set; }
        public string Address { get; set; }
        public string GSTRegistrationNo { get; set; }

        public virtual List<Products> Products { get; set; }

    }
}