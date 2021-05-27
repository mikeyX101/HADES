using HADES.Util;
using HADES.Util.ModelAD;
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
            // timer repeats call to Send email notification
 
                _timer = new Timer(
              (object state) => verifyExpirationForAllGroup(),
              null,
              TimeSpan.Zero,
#if DEBUG
                TimeSpan.FromSeconds(60)
#endif

#if RELEASE
                TimeSpan.FromHours(24)
#endif
            );
           
          

            return Task.CompletedTask;
        }

        private static void verifyExpirationForAllGroup() {
            
            try
            {
                Log.Information("The system verify the expiration date of the group in the active directory {Function}", "verifyExpirationForAllGroup()");

                // Get all the groups in the AD Root
                ADManager ad = new ADManager();
                List<GroupAD> groups = ad.getGroupsInRoot();

                foreach (var group in groups)
                {
                    //If the group is expire an email is send every 24h
                    if (group.ExpirationDate != null && group.ExpirationDate <= DateTime.Now)
                    {
                        EmailHelper.SendEmail(NotificationType.ExpirationDate, group, "", -1);
                    }

                    //Send one email if the group expire will expire in the next 15 days
                    if (group.ExpirationDate != null)
                    {
                        DateTime date = DateTime.Now.AddDays(15);
                        DateTime dateExp = (DateTime)group.ExpirationDate;
                        if (dateExp.Date.CompareTo(date.Date) == 0)
                        {
                            EmailHelper.SendEmail(NotificationType.ExpirationDate, group, "", 15);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Log.Warning(e, "An unexepected error occured while doing an operation with the Expiration Date Service in function {Function}", "verifyExpirationForAllGroup()");

            }
         
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
