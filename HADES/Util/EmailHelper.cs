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
using System.Threading;
using static HADES.Util.EmailHelper;

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

        public static void SendEmail(NotificationType type, GroupAD group, string usersAddedorDeleted = "", int nbExpirationDate = -1)
        {
            string groupGUID = group.ObjectGUID;
            List<string> emailsToNotifyFr = new List<string>();
            List<string> emailsToNotifyEng = new List<string>();
            List<string> emailsToNotifyEsp = new List<string>();
            List<string> emailsToNotifyPor = new List<string>();

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

                    switch (e.UserConfig.Language)
                    {
                        case "fr-CA":
                            emailsToNotifyFr.Add(e.Address);
                            break;
                        case "en-US":
                            emailsToNotifyEng.Add(e.Address);
                            break;
                        case "es-US":
                            emailsToNotifyEsp.Add(e.Address);
                            break;
                        case "pt-BR":
                            emailsToNotifyPor.Add(e.Address);
                            break;
                    }
                    //Is a AD user
                }
                else if (e.UserConfig.User != null)
                {
                    //Is a SuperAdmin or an Admin
                    if (e.UserConfig.User.Role.Id == (int)RolesID.SuperAdmin || e.UserConfig.User.Role.Id == (int)RolesID.Admin)
                    {
                        switch (e.UserConfig.Language)
                        {
                            case "fr-CA":
                                emailsToNotifyFr.Add(e.Address);
                                break;
                            case "en-US":
                                emailsToNotifyEng.Add(e.Address);
                                break;
                            case "es-US":
                                emailsToNotifyEsp.Add(e.Address);
                                break;
                            case "pt-BR":
                                emailsToNotifyPor.Add(e.Address);
                                break;
                        }
                    }
                    //Is owner og the group
                    else if (e.UserConfig.User.RoleId == (int)RolesID.Owner)
                    {
                        foreach (var g in e.UserConfig.User.OwnerGroupUsers)
                        {
                            if (g.OwnerGroup.GUID == groupGUID)
                            {
                                switch (e.UserConfig.Language)
                                {
                                    case "fr-CA":
                                        emailsToNotifyFr.Add(e.Address);
                                        break;
                                    case "en-US":
                                        emailsToNotifyEng.Add(e.Address);
                                        break;
                                    case "es-US":
                                        emailsToNotifyEsp.Add(e.Address);
                                        break;
                                    case "pt-BR":
                                        emailsToNotifyPor.Add(e.Address);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                ThreadEmail t = new ThreadEmail(type, group, usersAddedorDeleted, nbExpirationDate, emailsToNotifyFr, emailsToNotifyEng, emailsToNotifyEsp, emailsToNotifyPor);
                Thread thr = new Thread(new ThreadStart(t.sendMailOnThread));
                thr.Start(); 
            }
            catch (Exception e)
            {
                Log.Warning(e, "An unexepected error occured while doing an operation in the {Service}", "EmailHelper");
            }

        }

      
        private class ThreadEmail
        {
            private NotificationType type;
            private GroupAD group;
            private string usersAddedorDeleted;
            private int nbExpirationDate;
            private string subject;
            private string message;
            private List<string> emailsToNotifyFr;
            private List<string> emailsToNotifyEng;
            private List<string> emailsToNotifyEsp;
            private List<string> emailsToNotifyPor;

            public ThreadEmail(NotificationType type, GroupAD group, string usersAddedorDeleted, int nbExpirationDate, List<string> emailsToNotifyFr, List<string> emailsToNotifyEng, List<string> emailsToNotifyEsp, List<string> emailsToNotifyPor)
            {
                this.type = type;
                this.group = group;
                this.usersAddedorDeleted = usersAddedorDeleted;
                this.nbExpirationDate = nbExpirationDate;
                this.subject = "";
                this.message = "";
                this.emailsToNotifyFr = emailsToNotifyFr;
                this.emailsToNotifyEng = emailsToNotifyEng;
                this.emailsToNotifyEsp = emailsToNotifyEsp;
                this.emailsToNotifyPor = emailsToNotifyPor;
            }


            public string Subject { get => subject; set => subject = value; }
            public string Message { get => message; set => message = value; }
            public NotificationType Type { get => type; set => type = value; }
            public GroupAD Group { get => group; set => group = value; }
            public string UsersAddedorDeleted { get => usersAddedorDeleted; set => usersAddedorDeleted = value; }
            public int NbExpirationDate { get => nbExpirationDate; set => nbExpirationDate = value; }

            public List<string> EmailsToNotifyFr { get => emailsToNotifyFr; set => emailsToNotifyFr = value; }
            public List<string> EmailsToNotifyEng { get => emailsToNotifyEng; set => emailsToNotifyEng = value; }
            public List<string> EmailsToNotifyEsp { get => emailsToNotifyEsp; set => emailsToNotifyEsp = value; }
            public List<string> EmailsToNotifyPor { get => emailsToNotifyPor; set => emailsToNotifyPor = value; }

            public void sendMailOnThread()
            {
                //Save the Current 
                CultureInfo currentC = Strings.Culture;

                //FR
                if (this.EmailsToNotifyFr.Count > 0)
                {
                    Strings.Culture = new System.Globalization.CultureInfo("fr-CA");
                    this.setMsgVariable(this.type, this.group, this.usersAddedorDeleted, this.nbExpirationDate);
                    using (EmailSink sink = new(this.EmailsToNotifyFr, this.Subject))
                    {
                        sink.AddMessage(LogEventLevel.Information, this.Message);
                    }
                }


                //ENG
                if (this.EmailsToNotifyEng.Count > 0)
                {
                    Strings.Culture = new System.Globalization.CultureInfo("en-US");
                    this.setMsgVariable(this.type, this.group, this.usersAddedorDeleted, this.nbExpirationDate);
                    using (EmailSink sink = new(this.EmailsToNotifyEng, this.Subject))
                    {
                        sink.AddMessage(LogEventLevel.Information, this.Message);
                    }
                }

                //ESP
                if (this.EmailsToNotifyEsp.Count > 0)
                {
                    Strings.Culture = new System.Globalization.CultureInfo("es-US");
                    this.setMsgVariable(this.type, this.group, this.usersAddedorDeleted, this.nbExpirationDate);
                    using (EmailSink sink = new(this.EmailsToNotifyEsp, this.Subject))
                    {
                        sink.AddMessage(LogEventLevel.Information, this.Message);
                    }
                }

                //PT
                if (this.EmailsToNotifyPor.Count > 0)
                {
                    Strings.Culture = new System.Globalization.CultureInfo("pt-BR");
                    this.setMsgVariable(this.type, this.group, this.usersAddedorDeleted, this.nbExpirationDate);
                    using (EmailSink sink = new(this.EmailsToNotifyPor, this.Subject))
                    {
                        sink.AddMessage(LogEventLevel.Information, this.Message);
                    }
                }

                //End Clean up
                Strings.Culture = currentC;
            }

            // Set the right subject and the message with the type of the notification
            public void setMsgVariable(NotificationType type, GroupAD groupDn, string usersAddedorDeleted, int nbExpirationDate)
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
                            this.subject = Strings.email_ExpirationDateSubject + " (" + groupDn.SamAccountName + " )"; ;
                            this.message = "\n" + Strings.email_ExpirationDateMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_ExpirationDateSubject + ": " + groupDn.ExpirationDate.ToString();
                        }
                        else
                        {
                            this.subject = Strings.email_ExpirationDateSubject + " (" + groupDn.SamAccountName + " )"; ;
                            this.message = "\n" + Strings.email_ExpirationDateSoonMsg + " " + nbExpirationDate + " " + Strings.email_ExpirationDateSoonMsg02 + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_ExpirationDateSubject + ": " + groupDn.ExpirationDate.ToString();
                        }
                        break;
                    case NotificationType.GroupCreate:
                        this.subject = Strings.email_GroupCreateSubject + " (" + groupDn.SamAccountName + " )"; ;
                        this.message = "\n" + Strings.email_GroupCreateMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName;
                        break;
                    case NotificationType.GroupDelete:
                        this.subject = Strings.email_GroupDeleteSubject + " (" + groupDn.SamAccountName + " )"; ;
                        this.message = "\n" + Strings.email_GroupDeleteMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members;
                        break;
                    case NotificationType.MemberAdd:
                        this.subject = Strings.email_MemberAddSubject + " (" + groupDn.SamAccountName + " )";
                        this.message = "\n" + Strings.email_MemberAddMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_membersaddes + usersAddedorDeleted;
                        break;
                    case NotificationType.MemberRemoval:
                        this.subject = Strings.email_MemberRemovalSubject + " (" + groupDn.SamAccountName + " )"; ;
                        this.message = "\n" + Strings.email_MemberRemovalMessage + "\n" + Strings.email_name + " " + groupDn.SamAccountName + "\n" + Strings.email_Members + members + "\n" + Strings.email_membersSup + usersAddedorDeleted;
                        break;
                }
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
