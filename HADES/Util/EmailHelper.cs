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
        public static bool SendEmail(NotificationType type) {


            //SuperAdmin and Admin = All groups
            //Owner only group owner


            ApplicationDbContext db = new ApplicationDbContext();
            DbSet<Email> emails = null;
            switch (type) {
                case NotificationType.ExpirationDate:
                     emails = (DbSet<Email>)db.Email.Where(c => c.ExpirationDate == true && c.UserConfig.Notification == true);
                    break;
                case NotificationType.GroupCreate:
                    emails = (DbSet<Email>)db.Email.Where(c => c.GroupCreate == true && c.UserConfig.Notification == true);
                    break;
                case NotificationType.GroupDelete:
                     emails = (DbSet<Email>)db.Email.Where(c => c.GroupDelete == true && c.UserConfig.Notification == true);
                    break;
                case NotificationType.MemberAdd:
                     emails = (DbSet<Email>)db.Email.Where(c => c.MemberAdd == true && c.UserConfig.Notification == true);
                    break;
                case NotificationType.MemberRemoval:
                     emails = (DbSet<Email>)db.Email.Where(c => c.MemberRemoval == true && c.UserConfig.Notification == true);
                    break;

            }

            // EXAMPLE
            List<string> emailsTemp = new()
            {
                "allo@allo.allo",
                "yo@whad.up"
            };

            using (EmailSink sink = new(emailsTemp, "Someone broke everything"))
			{
                sink.AddMessage(LogEventLevel.Information, "Yeah, someone  F U C K E D  everything up.");
			}


            return true;
        }

        //TODO TO TEST
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
