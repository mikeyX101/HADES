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

        public UserAD()
        {
            this.SamAccountName = null;
            this.FirstName = null;
            this.LastName = null;
        }
        public UserAD(string samAccountName, string firstName, string lastName)
        {
            this.SamAccountName = samAccountName;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string SamAccountName { get => samAccountName; set => samAccountName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }

        public override string ToString()
        {
            return "[ samAccountName: "+ SamAccountName + ", Firstname: " + FirstName + ", Lastname: " + LastName +"]";
        }
    }
}
