using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HADES.Models
{
	public class UserConfigViewModel
    {
        public UserConfig UserConfig { get; set; }
        public List<Email> Emails { get; set; }

        public List<SelectListItem> Languages { get; set; }
    }
}
