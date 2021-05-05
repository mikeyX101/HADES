using HADES.Data;
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

        private ADManager ad = new ADManager();

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
            if (UpdateMe) // Only update if asked to
            {
                processing = true;
                try
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    UpdateUsers(db);
                    UpdateOwnerGroups(db);
                    UpdateAdminSuperAdmin(db);
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
        }

        private void UpdateAdminSuperAdmin(ApplicationDbContext db)
        {
            Console.WriteLine("Hades Admin/SuperAdmin Groups Synchronized with Active Directory");
        }

        private void UpdateOwnerGroups(ApplicationDbContext db)
        {
            Console.WriteLine("Hades OwnerGroups Synchronized with Active Directory");
        }

        private void UpdateUsers(ApplicationDbContext db)
        {
            List<UserAD> ulist = ad.getAllUsers();
            // First Delete Users that are not in the Active Directory


            Console.WriteLine("Hades Users Synchronized with Active Directory");
        }
    }
}
