using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
    public interface IUser
    {
        public int GetId();

        public string GetName();

        public Role GetRole();

        public UserConfig GetUserConfig();

        public ICollection<OwnerGroupUser> GetGroupsUser();
    }
}
