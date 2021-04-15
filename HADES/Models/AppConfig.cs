using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("AppConfig_ACF")]
    public class AppConfig
    {
        [Key]
        [Required]
        [Column("ACF_id")]
        public int Id { get; set; }

        [Required]
        [Column("ACF_active_directory")]
        public string ActiveDirectory { get; set; }
        
        [Column("ACF_company_name")]
        public string CompanyName { get; set; }
        
        [Column("ACF_company_logo_file")]
        public string CompanyLogoFile { get; set; }
        
        [Column("ACF_company_background_file")]
        public string CompanyBackgroundFile { get; set; }

        [Required]
        [Column("ACF_default_language")]
        public string DefaultLanguage { get; set; }
        
        [Column("ACF_SMTP")]
        public string SMTP { get; set; }

        [Required]
        [Column("ACF_log_delete_frequency")]
        public int LogDeleteFrequency { get; set; }

        [Required]
        [Column("ACF_log_max_file_size")]
        public int LogMaxFileSize { get; set; }


        public virtual ICollection<AdminGroup> AdminGroups { get; set; }
        public virtual ICollection<SuperAdminGroup> SuperAdminGroups { get; set; }
    }
}