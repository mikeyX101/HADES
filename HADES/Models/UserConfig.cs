using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("UserConfig_UCF")]
    public class UserConfig
    {
        [Key]
        [Required]
        [Column("UCF_id")]
        public int Id { get; set; }

        [Required]
        [Column("UCF_language")]
        public string Language { get; set; }

        [Required]
        [Column("UCF_theme_file")]
        public string ThemeFile { get; set; }

        [Required]
        [Column("UCF_notification")]
        public bool Notification { get; set; }


        public virtual DefaultUser DefaultUser { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
    }
}