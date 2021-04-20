using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
    [Table("ActiveDirectory_ADR")]
    public class ActiveDirectory
    {
        [Column("ADR_id")]
        [Key]
        [Required]
        public int Id { get; set; }

        [Column("ADR_port_number")]
        [Required]
        public int PortNumber { get; set; }

        [Column("ADR_server_address")]
        [Required]
        public string ServerAddress { get; set; }

        [Column("ADR_connection_filter")]
        [Required]
        public string ConnectionFilter { get; set; }

        [Column("ADR_base_dn")]
        [Required]
        public string BaseDN { get; set; }

        [Column("ADR_account_dn")]
        [Required]
        public string AccountDN { get; set; }

        [Column("ADR_password_dn")]
        [Required]
        public string PasswordDN { get; set; }

        [Column("ADR_sync_field")]
        [Required]
        public string SyncField { get; set; }


        public virtual AppConfig AppConfig { get; set; }
    }
}
