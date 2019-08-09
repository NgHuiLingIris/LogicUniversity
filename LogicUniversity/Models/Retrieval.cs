using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class Retrieval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RetrievalId { get; set; }
        public DateTime DateRetrieved { get; set; }
        public int StoreClerkId { get; set; }

        public virtual List<RetrievalDetail> RetrievalDetails { get; set; }

        //[ForeignKey("StoreClerkId")]
        public Employee StoreClerk { get; set; }
    }
}
