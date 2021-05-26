using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HADES.Models
{
	public class AppConfigViewModel
    {
        [FromForm]
        public ActiveDirectory ActiveDirectory { get; set; }

        [FromForm]
        public List<AdminGroup> AdminGroups { get; set; }
        [FromForm]
        public List<SuperAdminGroup> SuperAdminGroups { get; set; }
        [FromForm]
        public DefaultUser DefaultUser { get; set; }
        [FromForm]
        public AppConfig AppConfig { get; set; }
    }
}
