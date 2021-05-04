using HADES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
    public class UserConfigViewModel
    {
        public UserConfig UserConfig { get; set; }
        public List<Email> Emails { get; set; }
    }
}
