using HADES.Data;
using HADES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Globalization;
using HADES.Util.ModelAD;

namespace HADES.Util
{
    public enum NotificationType
    {
        ExpirationDate,
        GroupCreate,
        GroupDelete,
        MemberAdd,
        MemberRemoval
    }

    public static class EmailHelper
    {
        private static List<string> emailsToNotifyFr = new List<string>();
        private static List<string> emailsToNotifyEng = new List<string>();
        private static List<string> emailsToNotifyEsp = new List<string>();
        private static List<string> emailsToNotifyPor = new List<string>();
        private static string subject = "";
        private static string message = "";

        public static void SendEmail(NotificationType type,  GroupAD group, string usersAddedorDeleted, int nbExpirationDate)
        {
            string groupGUID = group.ObjectGUID;
            Console.WriteLine("yo " + groupGUID);

            ApplicationDbContext db = new ApplicationDbContext();
            List<Email> emails = null;
            switch (type)
            {
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
                if (e.UserConfig.DefaultUser != null)
                {
                    Console.WriteLine("Send Email to " + e.Address + " " + e.UserConfig.Language);
                    EmailHelper.addToList(e.Address, e.UserConfig.Language);
                    //Is a AD user
                }
                else if (e.UserConfig.User != null)
                {
                    //Is a SuperAdmin or an Admin
                    if (e.UserConfig.User.Role.Id == (int)RolesID.SuperAdmin || e.UserConfig.User.Role.Id == (int)RolesID.Admin)
                    {
                        Console.WriteLine("Send Email to " + e.Address + " " + e.UserConfig.Language);
                        EmailHelper.addToList(e.Address, e.UserConfig.Language);
                    }
                    //Is owner og the group
                    else if (e.UserConfig.User.RoleId == (int)RolesID.Owner)
                    {
                        foreach (var g in e.UserConfig.User.OwnerGroupUsers)
                        {
                            Console.WriteLine(g.OwnerGroup.GUID);
                            if (g.OwnerGroup.GUID == groupGUID)
                            {
                                Console.WriteLine("Send Email to (Owner)" + e.Address + " " + e.UserConfig.Language);
                                EmailHelper.addToList(e.Address, e.UserConfig.Language);
                            }
                        }
                    }
                }
            }

            //Save the Current 
            CultureInfo currentC = Strings.Culture;

            //FR
            if (EmailHelper.emailsToNotifyFr.Count > 0)
            {
                Strings.Culture = new System.Globalization.CultureInfo("fr-CA");
                EmailHelper.setMsgVariable(type,group, usersAddedorDeleted, nbExpirationDate);
                using (EmailSink sink = new(EmailHelper.emailsToNotifyFr, EmailHelper.subject))
                {
                    sink.AddMessage(LogEventLevel.Information, EmailHelper.message);
                }
            }


            //ENG
            if (EmailHelper.emailsToNotifyEng.Count > 0)
            {
                Strings.Culture = new System.Globalization.CultureInfo("en-US");
                EmailHelper.setMsgVariable(type, group, usersAddedorDeleted, nbExpirationDate);
                using (EmailSink sink = new(EmailHelper.emailsToNotifyEng, EmailHelper.subject))
                {
                    sink.AddMessage(LogEventLevel.Information, EmailHelper.message);
                }
            }

            //ESP
            if (EmailHelper.emailsToNotifyEsp.Count > 0)
            {
                Strings.Culture = new System.Globalization.CultureInfo("es-US");
                EmailHelper.setMsgVariable(type, group, usersAddedorDeleted, nbExpirationDate);
                using (EmailSink sink = new(EmailHelper.emailsToNotifyEsp, EmailHelper.subject))
                {
                    sink.AddMessage(LogEventLevel.Information, EmailHelper.message);
                }
            }

            //PT
            if (EmailHelper.emailsToNotifyPor.Count > 0)
            {
                Strings.Culture = new System.Globalization.CultureInfo("pt-BR");
                EmailHelper.setMsgVariable(type, group, usersAddedorDeleted, nbExpirationDate);
                using (EmailSink sink = new(EmailHelper.emailsToNotifyPor, EmailHelper.subject))
                {
                    sink.AddMessage(LogEventLevel.Information, EmailHelper.message);
                }
            }

            //End Clean up
            Strings.Culture = currentC;
            EmailHelper.emailsToNotifyFr = new List<string>();
            EmailHelper.emailsToNotifyEng = new List<string>();
            EmailHelper.emailsToNotifyEsp = new List<string>();
            EmailHelper.emailsToNotifyPor = new List<string>();
        }

        //Add the email to the list base on the language
        private static void addToList(string email, string langue)
        {
            switch (langue)
            {
                case "fr-CA":
                    EmailHelper.emailsToNotifyFr.Add(email);
                    break;
                case "en-US":
                    EmailHelper.emailsToNotifyEng.Add(email);
                    break;
                case "es-US":
                    EmailHelper.emailsToNotifyEsp.Add(email);
                    break;
                case "pt-BR":
                    EmailHelper.emailsToNotifyPor.Add(email);
                    break;
            }
        }

        // Set the right subject and the message with the type of the notification
        private static void setMsgVariable(NotificationType type, GroupAD groupDn, string usersAddedorDeleted, int nbExpirationDate)
        {
            String members = "";
            foreach (var m in groupDn.Members)
            {
                members += m.FirstName + " " + m.LastName + ", ";
            }


            switch (type)
            {
                case NotificationType.ExpirationDate:
                    if (nbExpirationDate == -1)
                    {
                        EmailHelper.subject = Strings.email_ExpirationDateSubject + " (" + groupDn.SamAccountName + " )"; ;
                        EmailHelper.message = "\n" + Strings.email_ExpirationDateMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_ExpirationDateSubject + ": " + groupDn.ExpirationDate.ToString();

                    }
                    else {
                        EmailHelper.subject = Strings.email_ExpirationDateSubject + " (" + groupDn.SamAccountName + " )"; ;
                        EmailHelper.message = "\n" + Strings.email_ExpirationDateSoonMsg + " " + nbExpirationDate +" " + Strings.email_ExpirationDateSoonMsg02  + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_ExpirationDateSubject + ": " + groupDn.ExpirationDate.ToString();
                    }
                    break;
                case NotificationType.GroupCreate:
                    EmailHelper.subject = Strings.email_GroupCreateSubject + " (" + groupDn.SamAccountName + " )"; ;
                    EmailHelper.message = "\n" + Strings.email_GroupCreateMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName;
                    break;
                case NotificationType.GroupDelete:
                    EmailHelper.subject = Strings.email_GroupDeleteSubject + " (" + groupDn.SamAccountName + " )"; ;
                    EmailHelper.message = "\n" + Strings.email_GroupDeleteMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members;
                    break;
                case NotificationType.MemberAdd:
                    EmailHelper.subject = Strings.email_MemberAddSubject + " (" + groupDn.SamAccountName + " )";
                    EmailHelper.message = "\n" + Strings.email_MemberAddMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_membersaddes + usersAddedorDeleted;
                    break;
                case NotificationType.MemberRemoval:
                    EmailHelper.subject = Strings.email_MemberRemovalSubject + " (" + groupDn.SamAccountName + " )"; ;
                    EmailHelper.message = "\n" + Strings.email_MemberRemovalMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_membersSup + usersAddedorDeleted;
                    break;
            }
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
                    NetworkCredentials = new NetworkCredential(settings.SMTPUsername, settings.SMTPPassword),
                    EmailSubject = subject,
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
