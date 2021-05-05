using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("SuperAdminGroup_SUG")]
    public class SuperAdminGroup
    {
        [Key]
        [Required]
        [Column("SUG_id")]
        public int Id { get; set; }

        [Required]
        [Column("SUG_guid")]
        public string GUID { get; set; }

        [ForeignKey("AppConfig")]
        [Column("SUG_ACF_id")]
        public int AppConfigId { get; set; }


        public virtual AppConfig AppConfig { get; set; }
    }
}