using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HADES.Models
{
	[Table("AdminGroup_ADG")]
    public class AdminGroup
    {
        [Key]
        [Required]
        [Column("ADG_id")]
        public int Id { get; set; }

        [Required]
        [Column("ADG_guid")]
        public string GUID { get; set; }

        [ForeignKey("AppConfig")]
        [Column("ADG_ACF_id")]
        public int AppConfigId { get; set; }


        public virtual AppConfig AppConfig { get; set; }
    }
}