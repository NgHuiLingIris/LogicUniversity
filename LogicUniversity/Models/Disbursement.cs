using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Disbursement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisbursementId { get; set; }
        public int RepresentativeId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateDisbursed { get; set; }

        public string Status { get; set; }
        public string CollectionPointId { get; set; }

        public virtual List<DisbursementDetail> DisbursementDetails { get; set; }

        [ForeignKey("RepresentativeId")]
        public Employee Representative { get; set; }

        [ForeignKey("CollectionPointId")]
        public CollectionPoint CollectionPoint { get; set; }
    }
}
