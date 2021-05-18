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
        public static void SendEmail(NotificationType type, string groupGUID) {



            //SuperAdmin and Admin = All groups
            //Owner only group owner


            ApplicationDbContext db = new ApplicationDbContext();
            List<Email> emails = null;
            switch (type) {
                case NotificationType.ExpirationDate:
                     emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.ExpirationDate == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.GroupCreate:
                    emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.GroupCreate == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.GroupDelete:
                     emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.GroupDelete == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.MemberAdd:
                     emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.Role).Where(c => c.MemberAdd == true && c.UserConfig.Notification == true).ToList();
                    break;
                case NotificationType.MemberRemoval:
                     emails = db.Email.Include(c => c.UserConfig).ThenInclude(c => c.DefaultUser).Include(c => c.UserConfig).ThenInclude(c => c.User).ThenInclude(c => c.OwnerGroupUsers).ThenInclude(c => c.OwnerGroup).Where(c => c.MemberRemoval == true && c.UserConfig.Notification == true).ToList();
                    break;
            }

            foreach (var e in emails)
            {
                Console.WriteLine("---------------------------------- "+ e.Address + "  " );
                //Is a DefautUser
                if (e.UserConfig.DefaultUser != null) {
                    Console.WriteLine("----------------------------------DefaultUser " + e.UserConfig.DefaultUser.RoleId);

                    //TODO: Send notification

                //Is a AD user
                } else if (e.UserConfig.User != null) {
                    Console.WriteLine("----------------------------------User " + e.UserConfig.User.RoleId);
                    if (e.UserConfig.User.RoleId == (int)RolesID.SuperAdmin || e.UserConfig.User.RoleId == (int)RolesID.Admin)
                    {
                        //TODO: Send notification
                    }
                    else if (e.UserConfig.User.RoleId == (int)RolesID.Owner)
                    {

                        //TODO: Send notification
                    }

                }
                
             
            }
        }
    }
}
