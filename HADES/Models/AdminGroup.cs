using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("AdminGroup_ADG")]
    public class AdminGroup
    {
        [Column("ADG_id")]
        public int Id { get; set; }

        [Column("ADG_sam_account_name")]
        public string SamAccount { get; set; }

        [ForeignKey("ADG_ACF_id")]
        public virtual AppConfig AppConfig { get; set; }
    }
}