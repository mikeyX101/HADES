using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HADES.Models
{
	[Table("Email_EMA")]
    public class Email
    {
        [Key]
        [Required]
        [Column("EMA_id")]
        public int Id { get; set; }

        [Required]
        [Column("EMA_email")]
        public string Address { get; set; }

        [Required]
        [Column("EMA_expiration_date")]
        public bool ExpirationDate { get; set; }

        [Required]
        [Column("EMA_group_create")]
        public bool GroupCreate { get; set; }

        [Required]
        [Column("EMA_group_delete")]
        public bool GroupDelete { get; set; }

        [Required]
        [Column("EMA_member_add")]
        public bool MemberAdd { get; set; }

        [Required]
        [Column("EMA_member_remove")]
        public bool MemberRemoval { get; set; }

        [ForeignKey("UserConfig")]
        [Column("EMA_UCF_id")]
        public int UserConfigId { get; set; }


        public virtual UserConfig UserConfig { get; set; }
    }
}