using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HADES.Models
{
	[Table("OwnerGroup_GRP")]
    public class OwnerGroup
    {
        [Key]
        [Required]
        [Column("GRP_id")]
        public int Id { get; set; }

        [Required]
        [Column("GRP_guid")]
        public string GUID { get; set; }
      

        public virtual ICollection<OwnerGroupUser> OwnerGroupUsers { get; set; }
    }
}