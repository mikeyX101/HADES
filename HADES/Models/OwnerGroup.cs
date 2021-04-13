using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("OwnerGroup_GRP")]
    public class OwnerGroup
    {
        [Column("GRP_id")]
        public int Id { get; set; }

        [Column("GRP_sam_account_name")]
        public string SamAccount { get; set; }
      

        public virtual ICollection<OwnerGroupUser> OwnerGroupUsers { get; set; }
    }
}