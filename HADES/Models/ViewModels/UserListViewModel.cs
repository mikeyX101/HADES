using System.Collections.Generic;

namespace HADES.Models
{
	public class UserListViewModel
    {
        public List<UserViewModel> ActiveUsers { get; set; }

        public List<UserViewModel> InactiveUsers { get; set; }

    }

    public class UserViewModel
    {
        public string GUID { get; set; }
        public string SamAccount { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public string OwnerOf { get; set; }

        public string MemberOf { get; set; }
    }
}
