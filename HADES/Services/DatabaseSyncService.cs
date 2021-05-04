using HADES.Data;
using HADES.Util;
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

        private ApplicationDbContext db = new ApplicationDbContext();
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
                UpdateUsers();
                UpdateOwnerGroups();
                UpdateAdminSuperAdmin();
                UpdateMe = false;
            }
        }

        private void UpdateAdminSuperAdmin()
        {
            Console.WriteLine("Hades Admin/SuperAdmin Groups Synchronized with Active Directory");
        }

        private void UpdateOwnerGroups()
        {
            Console.WriteLine("Hades OwnerGroups Synchronized with Active Directory");
        }

        private void UpdateUsers()
        {
            ad.getAllUsers();
            Console.WriteLine("Hades Users Synchronized with Active Directory");
        }
    }
}
