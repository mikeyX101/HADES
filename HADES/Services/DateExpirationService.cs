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
            Console.WriteLine("The system verify the expiration date of the group in the active directory");
            ADManager ad = new ADManager();
            // Get all the groups in the AD Root
            List<GroupAD> groups = ad.getGroupsInRoot();

            foreach (var group in groups)
            {
                //If the group is expire an email is send every 24h
                if (group.ExpirationDate != null && group.ExpirationDate <= DateTime.Now) {
                    Console.WriteLine("Group is Expired");
                    EmailHelper.SendEmail(NotificationType.ExpirationDate, group, "", -1);
                }

                //Send one email if the group expire will expire in the next 15 days
                if (group.ExpirationDate != null) {
                    DateTime date = DateTime.Now.AddDays(15);
                    DateTime dateExp = (DateTime)group.ExpirationDate;
                    if (dateExp.Date.CompareTo(date.Date) == 0)
                    {
                        Console.WriteLine("Group wil Expire in 15");
                        EmailHelper.SendEmail(NotificationType.ExpirationDate, group, "", 15);
                    }
                }
                
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
