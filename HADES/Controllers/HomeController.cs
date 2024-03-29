using HADES.Data;
using HADES.Models;
using HADES.Util;
using HADES.Util.Exceptions;
using HADES.Util.ModelAD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HADES.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None,NoStore = true)]
    public class HomeController : LocalizedController<HomeController>
    {
        private ADManager ad;
        private MainViewViewModel viewModel;

        public HomeController(IStringLocalizer<HomeController> localizer, ApplicationDbContext ctx) : base(localizer)
        {
            ad = new ADManager();
            viewModel = new MainViewViewModel();
        }

        [Authorize]
        // Returns the Main Application View parameter is the selected Folder (Clean up to remove unauthorized)
        public IActionResult MainView()
        {
            try
            {
                viewModel.ADRoot = ad.getRoot();
                if (!ConnexionUtil.CurrentUser(User).GetRole().AdCrudAccess) viewModel.ADRoot = CleanUpADRootforOwner(viewModel.ADRoot);
                viewModel.ADRoot = SortADRoot(viewModel.ADRoot);
                BuildRootTreeNode(viewModel.ADRoot); // conversion List<RootDataInformation> en TreeNode<string>
                viewModel.ADRootTreeNodeJson = TreeNodeToJson(viewModel.ADRootTreeNode); // conversion TreeNode<string> en Json
                viewModel.SelectedPath = "/" + viewModel.ADRoot[0].SamAccountName; // select root OU par d�faut
                viewModel.SelectedNodeName = viewModel.ADRoot[0].SamAccountName;
                viewModel.ExpandedNodesName = JsonConvert.SerializeObject(new string[] { viewModel.ADRoot[0].SamAccountName }, Formatting.Indented);

                viewModel.CreateButtonLabel = Localizer["CreateNewOU"];
                viewModel.EditLinkLabel = Localizer["Rename"];
                viewModel.DataTarget = "#OuCreate";

                return View(viewModel);
            }
            catch (ADException) // Connection � l'AD impossible
            {
                viewModel.ADConnectionError = Localizer["ADConnectionError"];
                return View(viewModel);
            }
        }

        private List<RootDataInformation> CleanUpADRootforOwner(List<RootDataInformation> adroot)
        {
            IUser u = ConnexionUtil.CurrentUser(User);
            List<OwnerGroupUser> groupusers = u.GetGroupsUser().ToList();
            List<RootDataInformation> newRoot = new List<RootDataInformation>();
            List<RootDataInformation> grpsonly = new List<RootDataInformation>();


            foreach (RootDataInformation r in adroot)
            {
                if (r.Path == null || r.Path?.Split("/").Length <= 2 || groupusers.Where(g => g.OwnerGroup.GUID == r.ObjectGUID).FirstOrDefault() != null)
                {
                    newRoot.Add(r);
                    if (r.Path?.Split("/").Length > 2)
                    {
                        grpsonly.Add(r);
                    }
                }
            }

            List<RootDataInformation> toremove = new List<RootDataInformation>();
            foreach (RootDataInformation t in newRoot)
            {
                if (t.Path?.Split("/").Length <= 2 && grpsonly.Where(g => g.Path == t.Path + "/" + t.SamAccountName).FirstOrDefault() == null)
                {
                    toremove.Add(t);
                }
            }
            foreach (RootDataInformation t in toremove)
            {
                newRoot.Remove(t);
            }

            return newRoot;

        }

        [HttpPost]
        [HttpGet]
        [Authorize]
        public IActionResult UpdateContent(string selectedPathForContent, string expandedNodeNames)
        {
            if (TempData["Error"] != null)
            {
                viewModel.Error = (string)TempData["Error"];
            }

            List<string> usersAD = new();
            ad.getAllUsers().ForEach(user => usersAD.Add(string.Format("{0} {1} ({2})", user.FirstName, user.LastName, user.SamAccountName)));
            usersAD.Sort();
            viewModel.UsersAD = JsonConvert.SerializeObject(usersAD);

            viewModel.SelectedPath = selectedPathForContent;
            viewModel.ExpandedNodesName = expandedNodeNames;
            viewModel.ADRoot = ad.getRoot();
            if (!ConnexionUtil.CurrentUser(User).GetRole().AdCrudAccess) viewModel.ADRoot = CleanUpADRootforOwner(viewModel.ADRoot);
            viewModel.ADRoot = SortADRoot(viewModel.ADRoot);
            BuildRootTreeNode(viewModel.ADRoot); // conversion List<RootDataInformation> en TreeNode<string>
            viewModel.ADRootTreeNodeJson = TreeNodeToJson(viewModel.ADRootTreeNode); // conversion TreeNode<string> en Json
            var split = viewModel.SelectedPath.Split('/');
            viewModel.SelectedNodeName = split[split.Length - 1];
            if (split.Length == 2)
            {
                viewModel.CreateButtonLabel = Localizer["CreateNewOU"];
                viewModel.EditLinkLabel = Localizer["Rename"];
                viewModel.DataTarget = "#OuCreate";
            }
            if (split.Length == 3)
            {
                viewModel.CreateButtonLabel = Localizer["CreateNewGroup"];
                viewModel.EditLinkLabel = Localizer["Edit"];
                viewModel.DataTarget = "#GroupCreate";
            }
            return PartialView("_Main", viewModel);
        }


        private void BuildRootTreeNode(List<RootDataInformation> adRoot)
        {
            TreeNode<string> ou = null;
            TreeNode<string> group = null;
            TreeNode<string> member = null;
            string[] path = null;
            foreach (var item in adRoot)
            {
                path = item.Path?.Split("/");

                if (viewModel.ADRootTreeNode == null)
                {
                    viewModel.ADRootTreeNode = new TreeNode<string>(item.SamAccountName);
                }

                if (path == null)
                {
                    viewModel.ADRootTreeNode = new TreeNode<string>(item.SamAccountName);
                }
                else if (path.Length == 2)
                {
                    ou = viewModel.ADRootTreeNode.AddChild(item.SamAccountName);

                }
                else if (path.Length == 3)
                {
                    group = ou.AddChild(item.SamAccountName);
                }
                else if (path.Length == 4)
                {
                    member = group.AddChild(item.SamAccountName);
                }
            }
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        private string TreeNodeToJson(TreeNode<string> treeNode)
        {
            return JsonConvert.SerializeObject(treeNode, Formatting.Indented,
                                                    new JsonSerializerSettings
                                                    {
                                                        NullValueHandling = NullValueHandling.Ignore,
                                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                    })
                                                .Replace("\"nodes\": []", "");
        }



        [HttpPost]
        [Authorize]
        public IActionResult Delete(MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            string DN = null;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (DN == null)
            {
                DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
                if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException();
            }
            var split = viewModel.SelectedPath.Split('/');
            var selectedNodeName = split.Length == 2 ? split[1] : split[2];
            // Delete OU, an empty OU has split.Length == 2
            if (split.Length == 2)
            {
                // at this point, the OU does not contain groups since split.Length == 2
                if (ad.deleteOU(DN))
                {
                    Serilog.Log.Information("Le dossier(OU) " + DN + " a �t� supprim�");
                }
            }
            // Delete Group, a Group has split.Length == 3
            if (split.Length == 3)
            {
                if (ad.deleteGroup(DN))
                {
                    Serilog.Log.Information("Le groupe " + DN + " a �t� supprim�");
                }
            }
            return RedirectToAction("UpdateContent", "Home", new
            {
                selectedPathForContent = viewModel.SelectedPath,
                expandedNodeNames = viewModel.ExpandedNodesName
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Rename(MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            string DN = null;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (DN == null)
            {
                DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
                if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException();
            }
            if (ModelState.IsValid)
            {
                if (ad.renameOU(DN, viewModel.NewName))
                {
                    Serilog.Log.Information("Le dossier(OU) " + DN + " a �t� renomm�");
                }
            }

            return RedirectToAction("UpdateContent", "Home", new
            {
                selectedPathForContent = viewModel.SelectedPath,
                expandedNodeNames = viewModel.ExpandedNodesName
            });
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CreateGroupModal([Bind("GroupAD, SelectedNodeName, SelectedContentName, SelectedPath, SelectedMembers, SelectedOwners, ExpandedNodesName")] MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            ApplicationDbContext db = new ApplicationDbContext();


            string[] split = viewModel.SelectedPath.Split('/');
            string selectedNodeName = split.Length == 2 ? split[1] : split[2];
            GroupAD group = viewModel.GroupAD;
            List<UserAD> members = GetSelectedUsers(viewModel);



            ModelState.Remove("NewName");

            if (ModelState.IsValid)
            {


                if (ad.createGroup(selectedNodeName, group, members))
                {
                    string DN = null;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (DN == null)
                    {
                        DN = FindDN(viewModel.SelectedPath, group.SamAccountName);
                        if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException();
                    }
                    
                    string guid = ad.getGroupGUIDByDn(DN);

                    List<string> selectedOwnersNames = DeserializeUsers(viewModel.SelectedOwners);

                    List<UserAD> ownersAD = ad.getAllUsers().Where(x => selectedOwnersNames.Contains(x.SamAccountName)).ToList();
                    List<User> ownersDB = new();
                    Role role = db.Role.Where(x => x.Name == "Owner").FirstOrDefault();


                    foreach (var owner in ownersAD)
                    {
                        if (db.User.Where(x => x.GUID == owner.ObjectGUID).Any())
                        {
                            ownersDB.Add(db.User.Where(x => x.GUID == owner.ObjectGUID).FirstOrDefault());
                        }
                        else
                            ownersDB.Add(new User { Attempts = 0, Date = DateTime.Now, GUID = owner.ObjectGUID, Role = role, UserConfig = new() });
                    }

                    OwnerGroup ownerGroup = new() { GUID = guid, OwnerGroupUsers = new List<OwnerGroupUser>() };

                    ownersDB.ForEach(user => ownerGroup.OwnerGroupUsers.Add(new OwnerGroupUser { User = user, OwnerGroup = ownerGroup }));


                    db.OwnerGroup.Add(ownerGroup);
                    db.SaveChanges();

                }
            }
            else
            {
                viewModel.Error = HADES.Strings.CreateOrEditOUGroupError + "<br>";
                viewModel.Error += string.Join("<br>", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                viewModel.Error += "<br>";
                TempData["Error"] = viewModel.Error;
            }
            return RedirectToAction("UpdateContent", "Home", new
            {
                selectedPathForContent = viewModel.SelectedPath,
                expandedNodeNames = viewModel.ExpandedNodesName
            });
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult EditGroupModal([Bind("GroupAD, SelectedNodeName, SelectedPath, BeforeEditMembers, SelectedMembers, OuGroup, SelectedOwners, ExpandedNodesName")] MainViewViewModel viewModel)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            GroupAD group = viewModel.GroupAD;
            string DN = null;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (DN == null)
            {
                DN = FindDN(viewModel.SelectedPath, viewModel.OuGroup);
                if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException();
            }
            GroupAD oldgroup = ad.getGroupInformation(DN);
            string guid = ad.getGroupGUIDByDn(DN);
            Dictionary<UserAD, Util.Action> updatedGroupMembers = UpdatedGroupMembersKeyValueActions(viewModel);

            OwnerGroup ownerGroup = db.OwnerGroup.Where(x => x.GUID == guid).Include(x => x.OwnerGroupUsers).FirstOrDefault();
            List<User> ownersDB = new List<User>();
            if (ConnexionUtil.CurrentUser(User).GetRole().AdCrudAccess)
            {
                List<string> selectedOwnersNames = DeserializeUsers(viewModel.SelectedOwners);

                List<UserAD> ownersAD = ad.getAllUsers().Where(x => selectedOwnersNames.Contains(x.SamAccountName)).ToList();
                ownersDB = new();
                Role role = db.Role.Where(x => x.Name == "Owner").FirstOrDefault();


                foreach (var owner in ownersAD)
                {
                    if (db.User.Where(x => x.GUID == owner.ObjectGUID).Any())
                    {
                        ownersDB.Add(db.User.Where(x => x.GUID == owner.ObjectGUID).FirstOrDefault());
                    }
                    else
                        ownersDB.Add(new User { Attempts = 0, Date = DateTime.Now, GUID = owner.ObjectGUID, Role = role, UserConfig = new() });
                }
            }

            if (!ConnexionUtil.CurrentUser(User).GetRole().AdCrudAccess)
            {
                group.SamAccountName = oldgroup.SamAccountName;
                group.Email = oldgroup.Email;
                group.ExpirationDate = oldgroup.ExpirationDate;
            }

            ModelState.Remove("NewName");
            if (viewModel.GroupAD.SamAccountName == oldgroup.SamAccountName)
            {
                ModelState.Remove("GroupAD.SamAccountName");
            }
            if (ModelState.IsValid)
            {
                DateTime dateExp = viewModel.GroupAD.ExpirationDate;
                if (ad.modifyGroup(DN, group, viewModel.SelectedNodeName, updatedGroupMembers))
                {
                    if (ConnexionUtil.CurrentUser(User).GetRole().AdCrudAccess)
                    {
                        ownerGroup.OwnerGroupUsers.Clear();
                        ownersDB.ForEach(user => ownerGroup.OwnerGroupUsers.Add(new OwnerGroupUser { User = user, OwnerGroup = ownerGroup }));

                        db.SaveChanges();
                    }

                    return RedirectToAction("UpdateContent", "Home", new
                    {
                        selectedPathForContent = viewModel.SelectedPath,
                        expandedNodeNames = viewModel.ExpandedNodesName
                    });
                }
            }
            else
            {
                viewModel.Error = HADES.Strings.CreateOrEditOUGroupError + "<br>";
                viewModel.Error += string.Join("<br>", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                viewModel.Error += "<br>";
                TempData["Error"] = viewModel.Error;
            }
            return RedirectToAction("UpdateContent", "Home", new
            {
                selectedPathForContent = viewModel.SelectedPath,
                expandedNodeNames = viewModel.ExpandedNodesName
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateOU(MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            if (ModelState.IsValid)
            {
                if (ad.createOU(viewModel.NewName))
                {
                    Serilog.Log.Information("Le dossier(OU) " + viewModel.NewName + " a �t� cr��");
                }
            }
            else
            {
                viewModel.Error = HADES.Strings.CreateOrEditOUGroupError + "<br>";
                viewModel.Error += string.Join("<br>", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                viewModel.Error += "<br>";
                TempData["Error"] = viewModel.Error;
            }
            return RedirectToAction("UpdateContent", "Home", new
            {
                selectedPathForContent = viewModel.SelectedPath,
                expandedNodeNames = viewModel.ExpandedNodesName
            });
        }

        private string FindDN(string selectedPath, string selectedContentName)
        {
            viewModel.ADRoot = ad.getRoot();
            return viewModel.ADRoot.Find(e => e.Path == selectedPath && e.SamAccountName == selectedContentName)?.Dn;
        }

        private List<RootDataInformation> SortADRoot(List<RootDataInformation> adRoot)
        {
            List<RootDataInformation> adRootSorted = new List<RootDataInformation>();

            adRootSorted = adRoot.OrderBy(data => data.Path + '/' + data.SamAccountName).ToList();

            return adRootSorted;
        }


        private Dictionary<UserAD, Util.Action> UpdatedGroupMembersKeyValueActions(MainViewViewModel viewModel)
        {
            List<string> beforeEditUsers = DeserializeUsers(viewModel.BeforeEditMembers);
            List<string> selectedUsers = DeserializeUsers(viewModel.SelectedMembers);

            IEnumerable<string> usersToDelete = beforeEditUsers.Except(selectedUsers);
            IEnumerable<string> usersToAdd = selectedUsers.Except(beforeEditUsers);

            Dictionary<UserAD, Util.Action> keyValueToDelete = ad.getAllUsers().Where(x => usersToDelete.Contains(x.SamAccountName)).ToDictionary(x => x, x => Util.Action.DELETE);
            Dictionary<UserAD, Util.Action> keyValueToAdd = ad.getAllUsers().Where(x => usersToAdd.Contains(x.SamAccountName)).ToDictionary(x => x, x => Util.Action.ADD);

            return keyValueToDelete.Concat(keyValueToAdd).ToDictionary(x => x.Key, x => x.Value);
        }


        private static List<String> DeserializeUsers(string serializedUsers)
        {
            List<string> deserializedUsers = serializedUsers != null ? JsonConvert.DeserializeObject<List<string>>(serializedUsers) : new();
            List<string> usersSamAccountName = new();
            int subFrom;
            int subTo;

            foreach (var user in deserializedUsers)
            {
                subFrom = user.IndexOf("(") + "(".Length;
                subTo = user.LastIndexOf(")") - subFrom;
                usersSamAccountName.Add(user.Substring(subFrom, subTo));
            }

            return usersSamAccountName;
        }


        public List<UserAD> GetSelectedUsers(MainViewViewModel viewModel)
        {
            List<UserAD> members = new();
            if (viewModel.SelectedMembers != null)
            {
                List<string> selectedUsers = DeserializeUsers(viewModel.SelectedMembers);
                members = ad.getAllUsers().Where(x => selectedUsers.Contains(x.SamAccountName)).ToList();
            }
            return members;
        }

        [Authorize]
        public IActionResult GetTabsContent(string dn, string selectedPath, string selectedNodeName, int index, string expandedNodesName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            GroupAD group = ad.getGroupInformation(dn);

            var usersDB = db.User.Where(x => x.OwnerGroupUsers.Select(x => x.OwnerGroup.GUID).Contains(group.ObjectGUID)).ToList();
            var owners = ad.getAllUsers().Where(user => usersDB.Select(x => x.GUID).Contains(user.ObjectGUID)).ToList();

            List<string> selectedMembers = new();
            List<string> selectedOwners = new();
            List<string> usersAD = new();

            group.Members.ForEach(member => selectedMembers.Add(string.Format("{0} {1} ({2})", member.FirstName, member.LastName, member.SamAccountName)));
            owners.ForEach(owner => selectedOwners.Add(string.Format("{0} {1} ({2})", owner.FirstName, owner.LastName, owner.SamAccountName)));
            ad.getAllUsers().ForEach(user => usersAD.Add(string.Format("{0} {1} ({2})", user.FirstName, user.LastName, user.SamAccountName)));

            selectedMembers.Sort();
            selectedOwners.Sort();
            usersAD.Sort();

            viewModel.UsersAD = JsonConvert.SerializeObject(usersAD);

            if (selectedMembers.Any())
            {
                viewModel.SelectedMembers = Newtonsoft.Json.JsonConvert.SerializeObject(selectedMembers);
                viewModel.BeforeEditMembers = Newtonsoft.Json.JsonConvert.SerializeObject(selectedMembers);
            }
            else
            {
                viewModel.SelectedMembers = "";
                viewModel.BeforeEditMembers = "";
            }

            if (selectedOwners.Any())
                viewModel.SelectedOwners = JsonConvert.SerializeObject(selectedOwners);
            else
                viewModel.SelectedOwners = "";

            viewModel.Index = index;
            viewModel.OuGroup = group.SamAccountName;
            viewModel.GroupAD = group;
            viewModel.SelectedPath = selectedPath;
            viewModel.SelectedNodeName = selectedNodeName;
            viewModel.ExpandedNodesName = expandedNodesName;

            return PartialView("_TabContentPartial", viewModel);
        }
    }

}
