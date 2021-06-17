using HADES.Data;
using HADES.Models;
using HADES.Util;
using HADES.Util.ModelAD;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HADES.Services
{
	public class DatabaseSyncService : IHostedService
    {
        private static Timer _timer;

        private static bool UpdateMe = false;
        private static bool processing = false;

        private static ADManager ad;

        // Flag the database for update
        public static void ExecUpdate()
        {
            UpdateMe = true;
        }

        // Forces an Update on the current thread
        public static void ForceUpdate()
        {
            UpdateDatabase(null, true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // timer repeats call to UpdateDatabase every 5 minutes.
            _timer = new Timer(
                (object state) => UpdateDatabase(state),
                null,
                TimeSpan.Zero,
#if DEBUG
                TimeSpan.FromSeconds(30)
#else
                TimeSpan.FromMinutes(5)
#endif
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private static void UpdateDatabase(object state, bool forced = false)
        {
            ApplicationDbContext db = new();
            if (db.Database.GetAppliedMigrations().Any())
			{
                if ((UpdateMe && !processing) || forced) // Only update if asked to
                {
                    Log.Information("Running {Service}", "Database Sync Service");
                    processing = true;
                    if (ad == null) ad = new ADManager(); // Initialize ad on first use

                    try
                    {

                        UpdateOwnerGroups(db);
                        UpdateAdminSuperAdmin(db);
                        UpdateUsers(db);
                        UpdateMe = false;
                        processing = false;
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e, "An unexepected error occured while doing an operation in the {Service}", "Database Sync Service");
                        UpdateMe = false;
                        processing = false;
                        return;
                    }

                    processing = false; // Just to be sure
                }
            }
            else
			{
                Log.Information("Tried to run {Service}, but migrations have not ran yet. Waiting for next interval.", "Database Sync Service");
			}
        }
        private static void UpdateAdminSuperAdmin(ApplicationDbContext db)
        {
            List<SuperAdminGroup> sulist = db.SuperAdminGroup.ToList();
            List<AdminGroup> alist = db.AdminGroup.ToList();

            // First Delete groups that are not in the Active Directory

            foreach (SuperAdminGroup su in sulist)
            {
                if (!ad.doesGroupExist(su.GUID))
                {
                    db.Remove(su);
                }
            }
            foreach (AdminGroup a in alist)
            {
                if (!ad.doesGroupExist(a.GUID))
                {
                    db.Remove(a);
                }
            }
            db.SaveChanges();
            Log.Information("Hades {Sync} Synchronized with Active Directory", "Admin / SuperAdmin Groups");
        }

        private static void UpdateOwnerGroups(ApplicationDbContext db)
        {
            List<OwnerGroup> oglist = db.OwnerGroup.ToList();
            List<GroupAD> grplist = ad.getGroupsInRoot();

            // Remove OwnerGroups not in ADroot
            foreach (OwnerGroup og in oglist)
            {
                if (grplist.Where(a => a.ObjectGUID.Equals(og.GUID)).SingleOrDefault() == null)
                {
                    db.OwnerGroup.Remove(og);
                }
            }

            // Add OwnerGroups to DB

            foreach (GroupAD grp in grplist)
            {
                if (oglist.Where(a => a.GUID.Equals(grp.ObjectGUID)).SingleOrDefault() == null)
                {
                    db.OwnerGroup.Add(new OwnerGroup { GUID = grp.ObjectGUID });
                }
            }

            db.SaveChanges();
            // Get all groups under root in ADManager
            Log.Information("Hades {Sync} Synchronized with Active Directory", "OwnerGroups");
        }

        private static void UpdateUsers(ApplicationDbContext db)
        {
            List<UserAD> ulist = ad.getAllUsers();
            List<User> dblist = db.User.Include(i => i.OwnerGroupUsers).ToList();

            List<AdminGroup> adminGroupsDB = db.AdminGroup.ToList();
            List<SuperAdminGroup> superadminGroupsDB = db.SuperAdminGroup.ToList();

            Role adminRole = db.Role.First(r => r.Id == (int)RolesID.Admin);
            Role superadminRole = db.Role.First(r => r.Id == (int)RolesID.SuperAdmin);
            Role inactiveRole = db.Role.First(r => r.Id == (int)RolesID.Inactive);

            // Generate AD User List of Admin
            List<UserAD> adminsAD = new List<UserAD>();
            foreach (AdminGroup ag in adminGroupsDB)
            {
                adminsAD.AddRange(ad.GetMembersOfGroup(ad.getGroupDnByGUID(ag.GUID), null));
            }
            // Generate AD User List of SuperAdmin
            List<UserAD> superadminsAD = new List<UserAD>();
            foreach (SuperAdminGroup sag in superadminGroupsDB)
            {
                superadminsAD.AddRange(ad.GetMembersOfGroup(ad.getGroupDnByGUID(sag.GUID), null));
            }

            // Add Users from AD Admin & AD SuperAdmin if not in DB
            foreach(UserAD adminU in adminsAD)
            {
                if(db.User.Where(u=>u.GUID == adminU.ObjectGUID).FirstOrDefault()==null)
                db.User.Add(new User() { GUID = adminU.ObjectGUID, UserConfig = new UserConfig(), RoleId = (int)RolesID.Admin});
            }
            foreach (UserAD superU in superadminsAD)
            {
                if (db.User.Where(u => u.GUID == superU.ObjectGUID).FirstOrDefault() == null)
                    db.User.Add(new User() { GUID = superU.ObjectGUID, UserConfig = new UserConfig(), RoleId = (int)RolesID.SuperAdmin });
            }

            foreach (User u in dblist)
            {
                // First Delete Users that are not in the Active Directory
                if (ulist.Where(a => a.ObjectGUID.Equals(u.GUID)).SingleOrDefault() == null)
                {
                    db.User.Remove(u);
                }
                else
                {
                    // --- Update is in AdminGroup / SuperAdminGroup ---

                    // If is in list set SuperAdmin else Lower Privilege
                    if (superadminsAD.Where(a => a.ObjectGUID == u.GUID).FirstOrDefault() != null)
                    {
                        db.User.Update(u);
                        u.RoleId = (int)RolesID.SuperAdmin;
                        continue;
                    }
                    else
                    {
                        db.User.Update(u);
                        u.RoleId = (int)RolesID.Admin;
                    }

                    // If is in list set Admin else Lower Privilege
                    if (adminsAD.Where(a => a.ObjectGUID == u.GUID).FirstOrDefault() != null)
                    {
                        db.User.Update(u);
                        u.RoleId = (int)RolesID.Admin;
                        continue;
                    }
                    else
                    {
                        db.User.Update(u);
                        u.RoleId = (int)RolesID.Owner;
                    }


                    // Update User is active
                    if (u.RoleId == (int)RolesID.Owner && u.OwnerGroupUsers.Count == 0)
                    {
                        db.User.Update(u);
                        u.RoleId = (int)RolesID.Inactive;
                    }
                    else if (u.RoleId == (int)RolesID.Inactive)
                    {
                        db.User.Update(u);
                        u.RoleId = (int)RolesID.Owner;
                    }
                }
            }

            db.SaveChanges();
            Log.Information("Hades {Sync} Synchronized with Active Directory", "Users");
        }
    }
}
