using HADES.Data;
using HADES.Models;
using HADES.Services;
using HADES.Util;
using HADES.Util.Exceptions;
using HADES.Util.ModelAD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Controllers
{
    public class UserListController : Controller
    {
        private readonly ApplicationDbContext db;
        private ADManager ad;

        public UserListController(ApplicationDbContext context)
        {
            db = context;
            ad = new ADManager();
        }

        [Authorize]
        public IActionResult UserList()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().UserListAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            UserListViewModel ulist = new UserListViewModel() { ActiveUsers = new List<UserViewModel>(), InactiveUsers = new List<UserViewModel>() };
            List<User> allUsers = db.User.Include(a => a.Role).Include(a => a.OwnerGroupUsers).ThenInclude(a => a.OwnerGroup).ToList();

            foreach (User u in allUsers)
            {
                if (u.RoleId != (int)RolesID.Inactive) // est inactif ?
                {
                    try
                    {
                        UserAD adu = ad.getUserAD(u.GUID, true);
                        // Build Owner Of string
                        string ownerof;
                        // Build Member Of string
                        string memberof;

                        BuildMemberStrings(out ownerof, out memberof, u);

                        ulist.ActiveUsers.Add(new UserViewModel() { GUID = u.GUID, FirstName = adu.FirstName, LastName = adu.LastName, Role = u.Role.Name, SamAccount = adu.SamAccountName, OwnerOf = ownerof, MemberOf = memberof });
                    }
                    catch (LoginException)
                    {
#if DEBUG
                        // If user is not found in AD insert it as Unknown User and continue
                        ulist.ActiveUsers.Add(new UserViewModel() { GUID = u.GUID, FirstName = "UNKNOWN", LastName = "UNKNOWN", Role = u.Role.Name, SamAccount = "GUID " + u.GUID + " not found in Active Directory it should be deleted at next synchronization" });
#endif     
                        continue;
                    }
                    catch (ADException)
                    {
                        ulist = null;
                        break;
                    }
                }
                else
                {
                    try
                    {
                        UserAD adu = ad.getUserAD(u.GUID, true);
                        // Build Owner Of string
                        string ownerof;
                        // Build Member Of string
                        string memberof;

                        BuildMemberStrings(out ownerof, out memberof, u);

                        ulist.InactiveUsers.Add(new UserViewModel() { GUID = u.GUID, FirstName = adu.FirstName, LastName = adu.LastName, Role = u.Role.Name, SamAccount = adu.SamAccountName, OwnerOf = ownerof, MemberOf = memberof });
                    }
                    catch (LoginException)
                    {
#if DEBUG
                        // If user is not found in AD insert it as Unknown User and continue
                        ulist.InactiveUsers.Add(new UserViewModel() { FirstName = "UNKNOWN", LastName = "UNKNOWN", Role = u.Role.Name, SamAccount = "GUID " + u.GUID + " not found in Active Directory it should be deleted at next synchronization" });
#endif
                        continue;
                    }
                    catch (ADException)
                    {
                        ulist = null;
                        break;
                    }
                }
            }

            return View(ulist);
        }

        private void BuildMemberStrings(out string ownerof, out string memberof, User u)
        {
            ownerof = "";
            memberof = "";
            foreach (OwnerGroupUser ogu in u.OwnerGroupUsers)
            {
                ownerof += ad.getGroupSamAccountNameByGUID(ogu.OwnerGroup.GUID) + ", ";
            }

            foreach (string names in ad.GetGroupsNameforUser(ad.getUserAD(u.GUID, true).Dn, null))
            {
                memberof += names + ", ";
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(string guid)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().UserListAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            db.User.Remove(db.User.Where(u => u.GUID == guid).FirstOrDefault());
            db.SaveChanges();

            return RedirectToAction("UserList");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Remove(string guid)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().UserListAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            User user = db.User.Include(u => u.OwnerGroupUsers).Where(u => u.GUID == guid).FirstOrDefault();

            // Remove Owner Group and set inactive
            db.Update(user);
            user.OwnerGroupUsers.Clear();
            user.RoleId = (int)RolesID.Inactive;
            db.SaveChanges();

            // Remove groups
            List<string> groupsDN = ad.GetGroupsDNforUser(user.GUID, null);
            List<string> useraslist = new List<string>() { ad.getUserAD(user.GUID, true).Dn };
            foreach (string group in groupsDN)
            {
                ad.deleteMemberToGroup(group,useraslist);
            }

            return RedirectToAction("UserList");
        }

    }

}
