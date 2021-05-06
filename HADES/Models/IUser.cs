using System.Collections.Generic;

namespace HADES.Models
{
	public interface IUser
    {
        public int GetId();

        public string GetName();

        public Role GetRole();

        public UserConfig GetUserConfig();

        public ICollection<OwnerGroupUser> GetGroupsUser();

        public bool IsDefaultUser();
    }
}
