namespace HADES.Util.ModelAD
{
	public class RootDataInformation
    {
        private string type;
        private string path;
        private string dn;
        private string samAccountName;
        private string objectGUID;

        public string Type { get => type; set => type = value; }
        public string Path { get => path; set => path = value; }
        public string Dn { get => dn; set => dn = value; }
        public string SamAccountName { get => samAccountName; set => samAccountName = value; }
        public string ObjectGUID { get => objectGUID; set => objectGUID = value; }

        public RootDataInformation(string type, string path, string dn, string samAccountName, string objectGUID)
        {
            Type = type;
            Path = path;
            Dn = dn;
            SamAccountName = samAccountName;
            this.ObjectGUID = objectGUID;
        }
        public RootDataInformation()
        {
            Type = null;
            Path = null;
            Dn = null;
            SamAccountName = null;
            this.ObjectGUID = null;
        }

        public override string ToString()
        {
            return "[Type: "+ Type + ", Path: " + Path + ", DN:  " + Dn + ", SamAccountName:  " + SamAccountName + ", ObjectGUID: " + ObjectGUID + "]";
        }
    }
}
