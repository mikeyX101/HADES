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

        [Required(ErrorMessage = "AddressRequired")]
        [EmailAddress(ErrorMessage = "AddressError")]
        [Column("EMA_email")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Column("EMA_expiration_date")]
        [Display(Name = "ExpirationDate")]
        public bool ExpirationDate { get; set; }

        [Required]
        [Column("EMA_group_create")]
        [Display(Name = "GroupCreate")]
        public bool GroupCreate { get; set; }

        [Required]
        [Column("EMA_group_delete")]
        [Display(Name = "GroupDelete")]
        public bool GroupDelete { get; set; }

        [Required]
        [Column("EMA_member_add")]
        [Display(Name = "MemberAdd")]
        public bool MemberAdd { get; set; }

        [Required]
        [Column("EMA_member_remove")]
        [Display(Name = "MemberRemoval")]
        public bool MemberRemoval { get; set; }

        [ForeignKey("UserConfig")]
        [Column("EMA_UCF_id")]
        public int UserConfigId { get; set; }


        public virtual UserConfig UserConfig { get; set; }
    }
}