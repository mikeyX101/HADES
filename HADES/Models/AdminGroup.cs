using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("AdminGroup_ADG")]
    public class AdminGroup
    {
        [Required]
        [Column("ADG_id")]
        public int Id { get; set; }

        [Required]
        [Column("ADG_sam_account_name")]
        public string SamAccount { get; set; }

        [ForeignKey("AppConfig")]
        [Column("ADG_ACF_id")]
        public int AppConfigId { get; set; }


        public virtual AppConfig AppConfig { get; set; }
    }
}