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
        private Timer _timer;

        private static bool UpdateMe = false;
        private static bool processing = false;

        private ADManager ad;

        // Flag the database for update
        public static void ExecUpdate()
        {
            UpdateMe = true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // timer repeats call to UpdateDatabase every 5 minutes.
            _timer = new Timer(
                UpdateDatabase,
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

        private void UpdateDatabase(object state)
        {
            if (UpdateMe && !processing) // Only update if asked to
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
                    Console.WriteLine("An error Occured while synchronizing the database");
                    processing = false;
                    return;
                }
            }
            processing = false; // Just to be sure
        }

        private void UpdateAdminSuperAdmin(ApplicationDbContext db)
        {
            // Get all groups in AD ?
            Console.WriteLine("Hades Admin/SuperAdmin Groups Synchronized with Active Directory");
        }

        private void UpdateOwnerGroups(ApplicationDbContext db)
        {
            // Get all groups under root in ADManager
            Console.WriteLine("Hades OwnerGroups Synchronized with Active Directory");
        }

        private void UpdateUsers(ApplicationDbContext db)
        {
            List<UserAD> ulist = ad.getAllUsers();
            List<User> dblist = db.User.ToList();

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

                }
            }

            db.SaveChanges();
            Console.WriteLine("Hades Users Synchronized with Active Directory");
        }
    }
}
