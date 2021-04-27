using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util.ModelAD
{
    public class GroupAD
    {

        private List<UserAD> members;
        private string description;
        private string email;
        private string notes;
        private string samAccountName;

        public GroupAD()
        {
            this.SamAccountName = null;
            this.Members = new List<UserAD>();
            this.Description = null;
            this.Email = null;
            this.Notes = null;
            
        }

        public GroupAD(string samAccountName, List<UserAD> members, string description, string email, string notes)
        {
            this.SamAccountName = samAccountName;
            this.Members = members;
            this.Description = description;
            this.Email = email;
            this.Notes = notes;
        }

        public List<UserAD> Members { get => members; set => members = value; }
        public string Description { get => description; set => description = value; }
        public string Email { get => email; set => email = value; }
        public string Notes { get => notes; set => notes = value; }
        public string SamAccountName { get => samAccountName; set => samAccountName = value; }

        public override string ToString()
        {
            string m = " ";
            for (int i = 0; i<Members.Count; i++) {
                m += Members[i] + " | ";
            }

            return "[ SamAccountName: " + SamAccountName + ", Members: " + m + ", Description: "+ Description + ", Email: " + Email + ", Notes: "+ Notes+ "]";
        }
    }
}
