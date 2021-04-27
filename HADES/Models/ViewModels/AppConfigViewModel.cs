using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
    public class AppConfigViewModel
    {
        public ActiveDirectory ActiveDirectory { get; set; }

        public ICollection<AdminGroup> AdminGroups { get; set; }

        public ICollection<SuperAdminGroup> SuperAdminGroups { get; set; }

        public DefaultUser DefaultUser { get; set; }   

        public AppConfig AppConfig { get; set; }
    }
}
