using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace testUML.Models
{
    [Table("UserConfig_UCF")]
    public class UserConfig
    {
        [Column("UCF_id")]
        public int Id { get; set; }

        [Column("UCF_language")]
        public string Language { get; set; }

        [Column("UCF_theme_file")]
        public string ThemeFile { get; set; }

        [Column("UCF_notification")]
        public bool Notification { get; set; }


        
        //public int UserID { get; set; }
        
        //public int DefaultUserID { get; set; }

        public virtual DefaultUser DefaultUser { get; set; }
        //[ForeignKey("User")]
        public virtual User User { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
    }
}