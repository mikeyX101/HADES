using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("SuperAdminGroup_SUG")]
    public class SuperAdminGroup
    {
        [Column("SUG_id")]
        public int Id { get; set; }

        [Column("SUG_sam_account_name")]
        public string SamAccount { get; set; }

        [ForeignKey("SUG_ACF_id")]
        public virtual AppConfig AppConfig { get; set; }
    }
}