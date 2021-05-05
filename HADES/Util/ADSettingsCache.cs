using HADES.Models;
using HADES.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util
{
    public class ADSettingsCache
    {
        public static ActiveDirectory Ad { get; set; }

        public async static void Refresh() {
            AppConfigService service = new AppConfigService();
            Ad = await service.getADInfo();
        }
    }
}
