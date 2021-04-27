using HADES.Data;
using HADES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task UpdateAppConfig(AppConfigViewModel viewModel)
        {
            db.Update(viewModel.ActiveDirectory);
            viewModel.AppConfig.ActiveDirectory = viewModel.ActiveDirectory;

            db.Update(viewModel.AppConfig);

            CreateDefaultUserIfNotExist(viewModel);
            //if (viewModel.AdminGroups != null)
            //{
            //    foreach (var group in viewModel.AdminGroups)
            //    {
            //        db.Update(group);
            //    }
            //}

            //if (viewModel.SuperAdminGroups != null)
            //{
            //    foreach (var group in viewModel.SuperAdminGroups)
            //    {
            //        db.Update(group);
            //    }
            //}

            await db.SaveChangesAsync();
        }

        public async void CreateDefaultUserIfNotExist(AppConfigViewModel viewModel)
        {
            var defaultUser = await db.DefaultUser.FirstOrDefaultAsync();
            if (defaultUser == null)
            {
                CreateRolesIfNotExist();
                var role = db.Role.Where(x => x.Name == "Super Admin").First();
                var userConfig = new UserConfig();
                db.Update(userConfig);
                db.Update(new DefaultUser
                {
                    UserName = viewModel.DefaultUser.UserName,
                    Password = viewModel.DefaultUser.Password,
                    Role = role,
                    UserConfig = userConfig
                }); ;
            }
            else
            {
                db.ChangeTracker.Clear();
                db.Update(viewModel.DefaultUser);
            }
        }


        public void CreateRolesIfNotExist()
        {
            if (!db.Role.Any())
            {
                db.Role.Add(new Role
                {
                    Id = 1,
                    Name = "Super Admin",
                    AppConfigAccess = true,
                    EventLogAccess = true,
                    UserListAccess = true,
                    DefineOwner = true,
                    AdCrudAccess = true
                });

                db.Role.Add(new Role
                {
                    Id = 2,
                    Name = "Admin",
                    AppConfigAccess = true,
                    EventLogAccess = true,
                    UserListAccess = true,
                    DefineOwner = true,
                    AdCrudAccess = true
                });
                db.Role.Add(new Role
                {
                    Id = 3,
                    Name = "Owner",
                    AppConfigAccess = true,
                    EventLogAccess = true,
                    UserListAccess = true,
                    DefineOwner = true,
                    AdCrudAccess = true
                });
            };


            db.SaveChanges();
        }

        public bool AppConfigExists(AppConfigViewModel viewModel)
        {
            return db.AppConfig.Any(x => x.Id == viewModel.AppConfig.Id);
        }

    }
}
