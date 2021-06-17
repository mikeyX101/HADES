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
	public class DateExpirationService: IHostedService
    {

        private static Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //timer repeats call to Send email notification
 
                _timer = new Timer(
              (object state) => verifyExpirationForAllGroup(),
              null,
              TimeSpan.Zero,
#if DEBUG
                TimeSpan.FromSeconds(60)
#else
                TimeSpan.FromHours(24)
#endif
            );
           
          

            return Task.CompletedTask;
        }

        private static void verifyExpirationForAllGroup() {
            Log.Information("Running {Service}", "Expiration Date Service");
            Data.ApplicationDbContext db = new();
            if (db.Database.GetAppliedMigrations().Any())
			{
                try
                {
                    // Get all the groups in the AD Root
                    ADManager ad = new(db);
                    List<GroupAD> groups = ad.getGroupsInRoot();

                    foreach (var group in groups)
                    {
                        //If the group is expire an email is send every 24h
                    if (group.ExpirationDate <= DateTime.Now)
                        {
                        EmailHelper.SendEmail(NotificationType.ExpirationDate, group);
                        }

                        //Send one email if the group expire will expire in the next 15 days
                   
                            DateTime date = DateTime.Now.AddDays(15);
                        if (group.ExpirationDate.Date.CompareTo(date.Date) == 0)
                            {
                            EmailHelper.SendEmail(NotificationType.ExpirationDate, group, nbExpirationDate: 15);
                            }
                   
                    }
                }
                catch (Exception e)
                {
                    Log.Warning(e, "An unexepected error occured while doing an operation in the {Service}", "Expiration Date Service");
                }
            }
            else
			{
                Log.Information("Tried to run {Service}, but migrations have not ran yet. Waiting for next interval.", "Expiration Date Service");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
