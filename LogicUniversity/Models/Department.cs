using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Department
    {
     [Key]
     public int DeptId { get; set; }
     public string DeptName { get; set; }
     public string CollectionLocationId { get; set; }
     public string ContactName { get; set; }
     public string TelephoneNo { get; set; }
     public string Fax { get; set; }
     
    }
}