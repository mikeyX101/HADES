using HADES.Data;
using HADES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Globalization;

namespace HADES.Util
{
    public enum NotificationType {
        ExpirationDate,
        GroupCreate,
        GroupDelete,
        MemberAdd,
        MemberRemoval
    } 
    public static class EmailHelper
    {
        public static void SendEmail(NotificationType type, string groupGUID) {



            //SuperAdmin and Admin = All groups
            //Owner only group owner
            List<string> emailsToNotify = new List<string>();
            string subject = "";
            string msg = "";


            ApplicationDbContext db = new ApplicationDbContext();
            List<Email> emails = null;
            switch (type) {
                case NotificationType.ExpirationDate:
                    emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.DefaultUser).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.OwnerGroupUsers).ThenInclude(c => c.OwnerGroup).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.ExpirationDate == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.GroupCreate:
                    emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.DefaultUser).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.OwnerGroupUsers).ThenInclude(c => c.OwnerGroup).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.GroupCreate == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.GroupDelete:
                    emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.DefaultUser).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.OwnerGroupUsers).ThenInclude(c => c.OwnerGroup).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.GroupDelete == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.MemberAdd:
                    emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.DefaultUser).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.OwnerGroupUsers).ThenInclude(c => c.OwnerGroup).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.MemberAdd == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.MemberRemoval:
                     emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.DefaultUser).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.OwnerGroupUsers).ThenInclude(c => c.OwnerGroup).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.MemberRemoval == true && c.UserConfig.Notification == true).ToList();
                    break;
            }


             foreach (var e in emails)
             {
                 //Is a DefautUser
                 if (e.UserConfig.DefaultUser != null) {
                    Console.WriteLine("Send Email to " + e.Address);
                    //emailsToNotify.Add(e.Address);
                 //Is a AD user
                 } else if (e.UserConfig.User != null) {
                    //Is a SuperAdmin or an Admin
                     if (e.UserConfig.User.Role.Id == (int)RolesID.SuperAdmin || e.UserConfig.User.Role.Id == (int)RolesID.Admin)
                     {
                        Console.WriteLine("Send Email to " + e.Address);
                      //  emailsToNotify.Add(e.Address);
                     } 
                     //Is owner og the group
                     else if (e.UserConfig.User.RoleId == (int)RolesID.Owner)
                     {
                        foreach (var g in e.UserConfig.User.OwnerGroupUsers)
                        {
                            Console.WriteLine(g.OwnerGroup.GUID);
                            if (g.OwnerGroup.GUID == groupGUID) {
                                Console.WriteLine("Send Email to (Owner)" + e.Address);
                              //emailsToNotify.Add(e.Address);
                            }
                        }
                     }
                 }  
             }

            CultureInfo currentC = Strings.Culture;
           

          //Strings.Culture = new System.Globalization.CultureInfo("en-US");
           

            // Send Emails
            using (EmailSink sink = new(emailsToNotify, type.ToString()))
            {
                sink.AddMessage(LogEventLevel.Information, "Yeah, someone  F U C K E D  everything up.");
            }

            Strings.Culture = currentC;
        }

   
        private sealed class EmailSink : IDisposable
		{

            private readonly Serilog.Core.Logger log;

            public EmailSink(IEnumerable<string> toEmails, string subject)
			{
                if (SMTPSettingsCache.SMTP == null)
                {
                    SMTPSettingsCache.Refresh();
                }
                SMTPSettings settings = SMTPSettingsCache.SMTP;

                string emails = "";
                foreach (string email in toEmails)
				{
                    emails += $"{email};";
				}
                Serilog.Sinks.Email.EmailConnectionInfo emailConfig = new Serilog.Sinks.Email.EmailConnectionInfo()
                {
                    EnableSsl = true,
                    MailServer = settings.SMTPServer,
                    Port = settings.SMTPPort,
                    EmailSubject = subject,
                    NetworkCredentials = new NetworkCredential(settings.SMTPUsername, settings.SMTPPassword),
                    FromEmail = settings.SMTPFromEmail,
                    ToEmail = emails
                };

                log = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Email(emailConfig,
                        mailSubject: subject,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{Exception}"
                    ).CreateLogger();
            }

            public void AddMessage(LogEventLevel logLevel, string message)
			{
				switch (logLevel)
				{
                    // Shouldn't be used
					case LogEventLevel.Verbose:
                        Log.Verbose(message);
                        log.Verbose(message);
                        break;
					case LogEventLevel.Debug:
                        Log.Debug(message);
                        log.Debug(message);
                        break;
					case LogEventLevel.Information:
                        Log.Information(message);
                        log.Information(message);
                        break;
					case LogEventLevel.Warning:
                        Log.Warning(message);
                        log.Warning(message);
                        break;
					case LogEventLevel.Error:
                        Log.Error(message);
                        log.Error(message);
                        break;
					case LogEventLevel.Fatal:
                        Log.Fatal(message);
                        log.Fatal(message);
                        break;
				}
			}

			public void Dispose()
			{
                log.Dispose();
			}
		}
    }
}
