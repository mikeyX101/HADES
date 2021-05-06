using HADES.Util;
using HADES.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column("USE_attempts")]
        public int Attempts { get; set; }

        [Required]
        [Column("USE_date")]
        public DateTime Date { get; set; }

        [Required]
        [Column("USE_guid")]
        public string GUID { get; set; }

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
            try
            {
               return new ADManager().getUserAD(GUID,true).SamAccountName;
            }
            catch (ADException)
            {
                return GUID;
            }
        }

        public Role GetRole()
        {
            return this.Role;
        }

        public UserConfig GetUserConfig()
        {
            return this.UserConfig;
        }

        // Always return false
        public bool IsDefaultUser()
        {
            return false;
        }
    }
}