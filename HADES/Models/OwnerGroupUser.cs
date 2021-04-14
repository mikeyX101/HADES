using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("OwnerGroupUser_GRU")]
    public class OwnerGroupUser
    {
        [Required]
        [Column("GRU_id")]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Column("GRU_USE_id")]
        public int UserId { get; set; }

        [ForeignKey("OwnerGroup")]
        [Column("GRU_GRP_id")]
        public int OwnerGroupId { get; set; }


        public virtual User User { get; set; }
        public virtual OwnerGroup OwnerGroup { get; set; }
    }
}