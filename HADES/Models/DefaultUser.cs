using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("DefaultUser_DUS")]
    public class DefaultUser
    {
        [Required]
        [Column("DUS_id")]
        public int Id { get; set; }

        [Required]
        [Column("DUS_username")]
        public string UserName { get; set; }

        [Required]
        [Column("DUS_password_hash")]
        public string Password { get; set; }

        [ForeignKey("Role")]
        [Column("DUS_ROL_id")]
        public int RoleId { get; set; }

        [ForeignKey("UserConfig")]
        [Column("DUS_UCF_id")]
        public int UserConfigId { get; set; }


        public virtual Role Role { get; set; }
        public virtual UserConfig UserConfig { get; set; }
    }
}