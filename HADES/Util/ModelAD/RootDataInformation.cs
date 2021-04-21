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
        private string name;
        private string dn;
        private string samAccountName;
        public RootDataInformation()
        {
            this.Type = null;
            this.Path = null;
            this.Name = null;
            this.Dn = null;
            this.SamAccountName = null;
        }
        public RootDataInformation(string type, string path, string name, string dn, string samAccountName)
        {
            this.Type = type;
            this.Path = path;
            this.Name = name;
            this.Dn = dn;
            this.SamAccountName = samAccountName; 
        }

        public string Type { get => type; set => type = value; }
        public string Path { get => path; set => path = value; }
        public string Name { get => name; set => name = value; }
        public string Dn { get => dn; set => dn = value; }
        public string SamAccountName { get => samAccountName; set => samAccountName = value; }

        public override string ToString()
        {
            return "[Type: "+ Type + ", Path: " + Path + ", Name: " + Name + ", DN:  " + Dn + ", SamAccountName:  " + SamAccountName + "]";
        }
    }
}
