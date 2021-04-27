using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util.ModelAD
{
    public class RootDataInformation
    {
        private string type;
        private string path;
        private string dn;
        private string samAccountName;

        public string Type { get => type; set => type = value; }
        public string Path { get => path; set => path = value; }
        public string Dn { get => dn; set => dn = value; }
        public string SamAccountName { get => samAccountName; set => samAccountName = value; }

        public RootDataInformation(string type, string path, string dn, string samAccountName)
        {
            Type = type;
            Path = path;
            Dn = dn;
            SamAccountName = samAccountName;
        }
        public RootDataInformation()
        {
            Type = null;
            Path = null;
            Dn = null;
            SamAccountName = null;
        }

        public override string ToString()
        {
            return "[Type: "+ Type + ", Path: " + Path + ", DN:  " + Dn + ", SamAccountName:  " + SamAccountName + "]";
        }
    }
}
