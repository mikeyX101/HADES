using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HADES.Models
{
	[Table("AppConfig_ACF")]
    public class AppConfig
    {
        [Key]
        [Required]
        [Column("ACF_id")]
        public int Id { get; set; }

        [Column("ACF_company_name")]
        public string CompanyName { get; set; }

        [Column("ACF_company_logo_file")]
        public string CompanyLogoFile { get; set; }

        [Column("ACF_company_background_file")]
        public string CompanyBackgroundFile { get; set; }

        [Required]
        [Column("ACF_default_language")]
        public string DefaultLanguage { get; set; }

        [Column("ACF_SMTP_server")]
        public string SMTPServer { get; set; }

        [Column("ACF_SMTP_port")]
        public int SMTPPort { get; set; }

        [Column("ACF_SMTP_username")]
        public string SMTPUsername { get; set; }

        [Column("ACF_SMTP_password")]
        public string SMTPPassword { get; set; }

        [Column("ACF_SMTP_from_email")]
        public string SMTPFromEmail { get; set; }

        [Required]
        [Column("ACF_log_delete_frequency")]
        public int LogDeleteFrequency { get; set; }

        [Required]
        [Column("ACF_log_max_file_size")]
        public int LogMaxFileSize { get; set; }

        public uint LogTotalMaxSize => (uint)(LogMaxFileSize * LogDeleteFrequency);

        [ForeignKey("ActiveDirectory")]
        [Column("ACF_ADR_id")]
        public int ActiveDirectoryId { get; set; }

        public virtual ActiveDirectory ActiveDirectory { get; set; }

        public virtual ICollection<AdminGroup> AdminGroups { get; set; }

        public virtual ICollection<SuperAdminGroup> SuperAdminGroups { get; set; }
    }
}