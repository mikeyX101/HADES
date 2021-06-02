using HADES.Data;
using HADES.Models;
using HADES.Util.ModelAD;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

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

        public static void SendEmail(NotificationType type,  GroupAD group, string usersAddedorDeleted = "", int nbExpirationDate = -1)
        {
            string groupGUID = group.ObjectGUID;

            ApplicationDbContext db = new ApplicationDbContext();
            List<Email> emails = null;
            IIncludableQueryable<Email, Role> query = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.DefaultUser).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.OwnerGroupUsers).ThenInclude(c => c.OwnerGroup).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role);
            switch (type)
            {
                case NotificationType.ExpirationDate:
                    emails = query.Where(c => c.ExpirationDate == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.GroupCreate:
                    emails = query.Where(c => c.GroupCreate == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.GroupDelete:
                    emails = query.Where(c => c.GroupDelete == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.MemberAdd:
                    emails = query.Where(c => c.MemberAdd == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.MemberRemoval:
                    emails = query.Where(c => c.MemberRemoval == true && c.UserConfig.Notification == true).ToList();
                    break;
            }


            foreach (var e in emails)
            {
                //Is a DefautUser
                if (e.UserConfig.DefaultUser != null)
                {
                    EmailHelper.addToList(e.Address, e.UserConfig.Language);
                    //Is a AD user
                }
                else if (e.UserConfig.User != null)
                {
                    //Is a SuperAdmin or an Admin
                    if (e.UserConfig.User.Role.Id == (int)RolesID.SuperAdmin || e.UserConfig.User.Role.Id == (int)RolesID.Admin)
                    {
                        EmailHelper.addToList(e.Address, e.UserConfig.Language);
                    }
                    //Is owner og the group
                    else if (e.UserConfig.User.RoleId == (int)RolesID.Owner)
                    {
                        foreach (var g in e.UserConfig.User.OwnerGroupUsers)
                        {
                            if (g.OwnerGroup.GUID == groupGUID)
                            {
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

            private readonly string SendToEmails = "";
            private string MessageContent { get; set; } = "";

            public EmailSink(IEnumerable<string> toEmails, string subject)
            {
                if (SMTPSettingsCache.SMTP == null)
                {
                    SMTPSettingsCache.Refresh();
                }
                SMTPSettings settings = SMTPSettingsCache.SMTP;

                foreach (string email in toEmails)
                {
                    SendToEmails += $"{email};";
                }
                Serilog.Sinks.Email.EmailConnectionInfo emailConfig = new Serilog.Sinks.Email.EmailConnectionInfo()
                {
                    EnableSsl = true,
                    MailServer = settings.SMTPServer,
                    Port = settings.SMTPPort,
                    NetworkCredentials = 
                        string.IsNullOrWhiteSpace(settings.SMTPUsername) || string.IsNullOrWhiteSpace(settings.SMTPPassword) ? 
                        null : 
                        new NetworkCredential(settings.SMTPUsername, EncryptionUtil.Decrypt(settings.SMTPPassword)),
                    EmailSubject = subject,
                    FromEmail = settings.SMTPFromEmail,
                    ToEmail = SendToEmails
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
                MessageContent += $"{message}\n";
                switch (logLevel)
                {
                    // Shouldn't be used
                    case LogEventLevel.Verbose:
                        log.Verbose(message);
                        break;
                    case LogEventLevel.Debug:
                        log.Debug(message);
                        break;
                    case LogEventLevel.Information:
                        log.Information(message);
                        break;
                    case LogEventLevel.Warning:
                        log.Warning(message);
                        break;
                    case LogEventLevel.Error:
                        log.Error(message);
                        break;
                    case LogEventLevel.Fatal:
                        log.Fatal(message);
                        break;
                }
            }

            public void Dispose()
            {
                Log.Information("Sending email to recipients ({SentTo}) with content: {EmailContent}", SendToEmails, MessageContent);
                log.Dispose();
            }
        }
    }
}
