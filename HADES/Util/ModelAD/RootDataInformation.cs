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
        public RootDataInformation()
        {
            this.Type = null;
            this.Path = null;
            this.Name = null;
            this.Dn = null;
        }
        public RootDataInformation(string type, string path, string name, string dn)
        {
            this.Type = type;
            this.Path = path;
            this.Name = name;
            this.Dn = dn;
        }

        public string Type { get => type; set => type = value; }
        public string Path { get => path; set => path = value; }
        public string Name { get => name; set => name = value; }
        public string Dn { get => dn; set => dn = value; }

        public override string ToString()
        {
            return "[Type: "+ Type + ", Path: " + Path + ", Name: " + Name + ", DN:  " + Dn +"]";
        }
    }
}
