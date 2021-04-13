using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace testUML.Models
{
    [Table("DefaultUser_DUS")]
    public class DefaultUser
    {
        [Column("DUS_id")]
        public int Id { get; set; }

        [Column("DUS_username")]
        public string UserName { get; set; }

        [Column("DUS_password_hash")]
        public string Password { get; set; }

        [ForeignKey("DUS_ROL_id")]
        public virtual Role Role { get; set; }
        [ForeignKey("DUS_UCF_id")]
        public virtual UserConfig UserConfig { get; set; }
    }
}