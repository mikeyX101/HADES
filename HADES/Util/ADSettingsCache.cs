using HADES.Models;
using HADES.Services;

namespace HADES.Util
{
	public class ADSettingsCache
    {
        public static ActiveDirectory Ad { get; set; }

        public async static void Refresh() {
            AppConfigService service = new();
            Ad = await service.getADInfo();
        }

        public async static void Refresh(Data.ApplicationDbContext db)
        {
            AppConfigService service = new(db);
            Ad = await service.getADInfo();
        }
    }
}
