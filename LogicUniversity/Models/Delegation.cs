using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Delegation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DelegationId { get; set; }
        public int EmployeeId { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "StartDate is required")]
        [Display(Name = "StartDate")]
        public string StartDate { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "EndDate is required")]
        [Display(Name = "EndDate")]
        public string EndDate { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}