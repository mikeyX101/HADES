using HADES.Data;
using HADES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

           

            //TODO: Add serilogCode here



            return true;
        }
    }
}
