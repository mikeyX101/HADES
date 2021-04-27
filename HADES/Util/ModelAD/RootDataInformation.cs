using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util.ModelAD
{
    public class RootDataInformation
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Dn { get; set; }

        public override string ToString()
        {
            return "[Type: "+ Type + ", Path: " + Path + ", Name: " + Name + ", DN:  " + Dn + ", SamAccountName:  " + SamAccountName + "]";
        }
    }
}
