using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("User_USE")]
    public class User : IUser
    {
        [Key]
        [Required]
        [Column("USE_id")]
        public int Id { get; set; }

        [Required]
        [Column("USE_sam_account_name")]
        public string SamAccount { get; set; }

        [ForeignKey("Role")]
        [Column("USE_ROL_id")]
        public int RoleId { get; set; }

        [ForeignKey("UserConfig")]
        [Column("USE_UCF_id")]
        public int UserConfigId { get; set; }


        public virtual Role Role { get; set; }
        public virtual ICollection<OwnerGroupUser> OwnerGroupUsers { get; set; }
        public virtual UserConfig UserConfig { get; set; }

        public ICollection<OwnerGroupUser> GetGroupsUser()
        {
            return this.OwnerGroupUsers;
        }

        public int GetId()
        {
            return this.Id;
        }

        public string GetName()
        {
            return this.SamAccount;
        }

        public Role GetRole()
        {
            return this.Role;
        }

        public UserConfig GetUserConfig()
        {
            return this.UserConfig;
        }
    }
}