using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class CollectionPoint
    {

        [Key]
        public string CollectionPointId { get; set; }
        [DisplayName("Collection Point")]
        public string LocationName { get; set; }
        public string Time { get; set; }
        public string Day { get; set; }
        public int StoreClerkId { get; set; }

        [ForeignKey("StoreClerkId")]
        public Employee Employee { get; set; }
    }
}