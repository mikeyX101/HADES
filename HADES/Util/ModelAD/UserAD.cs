using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util.ModelAD
{
    public class UserAD
    {
        private string samAccountName;
        private string firstName;
        private string lastName;
        private string dn;

        public UserAD()
        {
            this.SamAccountName = null;
            this.FirstName = null;
            this.LastName = null;
            this.dn = null;
        }
        public UserAD(string samAccountName, string firstName, string lastName, string dn)
        {
            this.SamAccountName = samAccountName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Dn = dn;
        }

        public string SamAccountName { get => samAccountName; set => samAccountName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Dn { get => dn; set => dn = value; }

        public override string ToString()
        {
            return "[ samAccountName: "+ SamAccountName + ", Firstname: " + FirstName + ", Lastname: " + LastName + ", DN: " + dn + "]";
        }
    }
}
