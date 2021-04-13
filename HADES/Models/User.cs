using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("User_USE")]
    public class User
    {
        [Column("USE_id")]
        public int Id { get; set; }

        [Column("USE_sam_account_name")]
        public string SamAccount { get; set; }


        [ForeignKey("USE_ROL_id")]
        public virtual Role Role { get; set; }
        public virtual ICollection<OwnerGroupUser> OwnerGroupUsers { get; set; }
        [ForeignKey("USE_UCF_id")]
        public virtual UserConfig UserConfig { get; set; }
    }
}