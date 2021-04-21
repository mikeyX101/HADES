using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
    public class AppConfigViewModel
    {
        public ActiveDirectory ActiveDirectory { get; set; }

        public SuperAdminGroup SuperAdminGroup { get; set; }

        public DefaultUser DefaultUser { get; set; }

        public AdminGroup AdminGroup { get; set; }

        public string CompanyLogoFile { get; set; }

        public string CompanyName { get; set; }

        public string CompanyBackgroundFile { get; set; }

        public string DefaultLanguage { get; set; }

        public string SMTP { get; set; }

        public int LogDeleteFrequency { get; set; }

        public int LogMaxFileSize { get; set; }

        public int LogTotalMaxSize => LogMaxFileSize * LogDeleteFrequency;

    }
}
