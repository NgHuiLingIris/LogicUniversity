using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Requisition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequisitionId { get; set; }
        public int? EmployeeId { get; set; }
        public string Department { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public int? ApproverId { get; set; }
        public string Remarks { get; set; }

        public virtual List<RequisitionDetails> RequisitionDetails { get; set; }
    }
}