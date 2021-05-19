using HADES.Data;
using HADES.Models;
using HADES.Util;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Services
{
	public class AppConfigService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<AppConfigViewModel> AppConfigViewModelGET()
        {
            var appConfig = await db.AppConfig.FirstOrDefaultAsync();
            var activeDirectory = await db.ActiveDirectory.FirstOrDefaultAsync();
            var defaultUser = await db.DefaultUser.FirstOrDefaultAsync();

            List<AdminGroup> adminGroups = null;
            List<SuperAdminGroup> superAdminGroups = null;

            if (appConfig != null)
            {
                adminGroups = await db.AdminGroup.Where(x => x.AppConfigId == appConfig.Id).ToListAsync();
                superAdminGroups = await db.SuperAdminGroup.Where(x => x.AppConfigId == appConfig.Id).ToListAsync();
            }

            AppConfigViewModel viewModel = new AppConfigViewModel
            {
                AppConfig = appConfig,
                ActiveDirectory = activeDirectory,
                AdminGroups = adminGroups,
                SuperAdminGroups = superAdminGroups,
                DefaultUser = defaultUser
            };

            return viewModel;
        }

        public AppConfig GetAppConfig()
        {
            return db.AppConfig.FirstOrDefault();
        }

        public async Task<ActiveDirectory> getADInfo()
        {
            var activeDirectory = await db.ActiveDirectory.FirstOrDefaultAsync();
            return activeDirectory;
        }

        public async Task<SMTPSettings> getSMTPInfo()
        {
            SMTPSettings settings = await db.AppConfig.Select(app => 
                new SMTPSettings(app.SMTPServer, app.SMTPPort, app.SMTPUsername, app.SMTPPassword, app.SMTPFromEmail)
            ).FirstOrDefaultAsync();
            return settings;
        }

        public async Task UpdateAppConfig(AppConfigViewModel viewModel)
        {
            db.Update(viewModel.ActiveDirectory);
            viewModel.AppConfig.ActiveDirectory = viewModel.ActiveDirectory;

            db.Update(viewModel.AppConfig);
            db.Update(viewModel.DefaultUser);
            if (viewModel.AdminGroups != null)
            {
                foreach (var group in viewModel.AdminGroups)
                {
                    db.Update(group);
                }
            }

            if (viewModel.SuperAdminGroups != null)
            {
                foreach (var group in viewModel.SuperAdminGroups)
                {
                    db.Update(group);
                }
            }

            
            await db.SaveChangesAsync();

            // Make these functions async and run them at the same time using Task.WaitAll()?
            LogManager.RefreshLogger(viewModel.AppConfig);
            ADSettingsCache.Refresh();
            SMTPSettingsCache.Refresh();
        }

        public bool AppConfigExists(AppConfigViewModel viewModel)
        {
            return db.AppConfig.Any(x => x.Id == viewModel.AppConfig.Id);
        }

        public async Task AddAdminGroup(AdminGroup adminGroup)
        {
            db.Add(adminGroup);
            await db.SaveChangesAsync();
        }

        public async Task AddSuperAdminGroup(SuperAdminGroup SuperAdminGroup)
        {
            db.Add(SuperAdminGroup);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAdminGroup(int? id)
        {
            var adminGroup = await db.AdminGroup.FindAsync(id);
            db.AdminGroup.Remove(adminGroup);
            await db.SaveChangesAsync();
        }

        public async Task DeleteSuperAdminGroup(int? id)
        {
            var superAdminGroup = await db.SuperAdminGroup.FindAsync(id);
            db.SuperAdminGroup.Remove(superAdminGroup);
            await db.SaveChangesAsync();
        }

        public async Task<int> AdminGroupRedirectId(int? id)
        {
            var adminGroup = await db.AdminGroup.FindAsync(id);
            return adminGroup.AppConfigId;
        }

        public async Task<int> SuperAdminGroupRedirectId(int? id)
        {
            var superAdminGroup = await db.SuperAdminGroup.FindAsync(id);
            return superAdminGroup.AppConfigId;
        }


    }
}
