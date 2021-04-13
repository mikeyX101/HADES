using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace testUML.Models
{
    [Table("OwnerGroupUser_GRU")]
    public class OwnerGroupUser
    {
        [Column("GRU_id")]
        public int Id { get; set; }


        [ForeignKey("GRU_USE_id")]
        public virtual User User { get; set; }
        [ForeignKey("GRU_GRP_id")]
        public virtual OwnerGroup OwnerGroup { get; set; }
    }
}