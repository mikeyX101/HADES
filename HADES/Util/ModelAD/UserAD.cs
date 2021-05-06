namespace HADES.Util.ModelAD
{
	public class UserAD
    {
        private string samAccountName;
        private string firstName;
        private string lastName;
        private string dn;
        private string objectGUID;

        public UserAD()
        {
            this.SamAccountName = null;
            this.FirstName = null;
            this.LastName = null;
            this.Dn = null;
            this.ObjectGUID = null;
        }
        public UserAD(string samAccountName, string firstName, string lastName, string dn, string objectGUID)
        {
            this.SamAccountName = samAccountName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Dn = dn;
            this.ObjectGUID = objectGUID;
        }

        public string SamAccountName { get => samAccountName; set => samAccountName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Dn { get => dn; set => dn = value; }
        public string ObjectGUID { get => objectGUID; set => objectGUID = value; }

        public override string ToString()
        {
            return "[ samAccountName: "+ SamAccountName + ", Firstname: " + FirstName + ", Lastname: " + LastName + ", DN: " + dn + ", ObjectGUID: " + ObjectGUID + "]";
        }
    }
}
