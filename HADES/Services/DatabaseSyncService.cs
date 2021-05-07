using HADES.Data;
using HADES.Models;
using HADES.Util;
using HADES.Util.ModelAD;
using Microsoft.Extensions.Hosting;
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
            UpdateDatabase(null,true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // timer repeats call to UpdateDatabase every 5 minutes.
            _timer = new Timer(
                (object state)=>UpdateDatabase(state),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(5)
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private static void UpdateDatabase(object? state, bool forced = false)
        {
            if ((UpdateMe && !processing)||forced) // Only update if asked to
            {
                processing = true;
                if (ad == null) ad = new ADManager(); // Initialize ad on first use

                try
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    UpdateOwnerGroups(db);
                    UpdateAdminSuperAdmin(db);
                    UpdateUsers(db);
                    UpdateMe = false;
                    processing = false;
                }
                catch (Exception)
                {
                    Console.WriteLine("An error Occured while synchronizing the HADES database to the Active Directory");
                    processing = false;
                    return;
                }
            }
            processing = false; // Just to be sure
        }

        private static void UpdateAdminSuperAdmin(ApplicationDbContext db)
        {
            List<SuperAdminGroup> sulist = db.SuperAdminGroup.ToList();
            List<AdminGroup> alist = db.AdminGroup.ToList();

            // First Delete groups that are not in the Active Directory

            foreach(SuperAdminGroup su in sulist)
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

            // Get all groups in AD ?
            Console.WriteLine("Hades Admin/SuperAdmin Groups Synchronized with Active Directory");
        }

        private static void UpdateOwnerGroups(ApplicationDbContext db)
        {
            List<OwnerGroup> oglist = db.OwnerGroup.ToList();

            // Get all groups under root in ADManager
            Console.WriteLine("Hades OwnerGroups Synchronized with Active Directory");
        }

        private static void UpdateUsers(ApplicationDbContext db)
        {
            List<UserAD> ulist = ad.getAllUsers();
            List<User> dblist = db.User.ToList();

            Role adminRole = db.Role.First(r => r.Id == (int)RolesID.Admin);
            Role superadminRole = db.Role.First(r => r.Id == (int)RolesID.SuperAdmin);
            Role inactiveRole = db.Role.First(r => r.Id == (int)RolesID.Inactive);

            foreach (User u in dblist)
            {
                // First Delete Users that are not in the Active Directory
                if(ulist.Where(a => a.ObjectGUID.Equals(u.GUID)).SingleOrDefault()==null)
                {
                    db.User.Remove(u);
                }
                else
                {
                    // Update is in AdminGroup / SuperAdminGroup
                    
                    // Update User is active
                }
            }

            db.SaveChanges();
            Console.WriteLine("Hades Users Synchronized with Active Directory");
        }
    }
}
