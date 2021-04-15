using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("Role_ROL")]
    public class Role
    {
        [Key]
        [Required]
        [Column("ROL_id")]
        public int Id { get; set; }

        [Required]
        [Column("ROL_name")]
        public string Name { get; set; }

        [Required]
        [Column("ROL_access_app_config")]
        public bool AppConfigAccess { get; set; }

        [Required]
        [Column("ROL_access_event_log")]
        public bool EventLogAccess { get; set; }

        [Required]
        [Column("ROL_access_users_list")]
        public bool UserListAccess { get; set; }

        [Required]
        [Column("ROL_define_owner")]
        public bool DefineOwner { get; set; }

        [Required]
        [Column("ROL_access_ad_crud")]
        public bool AdCrudAccess { get; set; }


        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<DefaultUser> DefaultUsers { get; set; }
    }

}