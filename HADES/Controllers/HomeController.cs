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
using System.Linq;

namespace HADES.Controllers
{
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
                viewModel.ADManager = ad;
                viewModel.ADRoot = ad.getRoot();
                viewModel.ADRoot = SortADRoot(viewModel.ADRoot);
                BuildRootTreeNode(viewModel.ADRoot); // conversion List<RootDataInformation> en TreeNode<string>
                viewModel.ADRootTreeNodeJson = TreeNodeToJson(viewModel.ADRootTreeNode); // conversion TreeNode<string> en Json
                viewModel.SelectedPath = "/" + viewModel.ADRoot[0].SamAccountName; // select root OU par défaut
                viewModel.SelectedNodeName = viewModel.ADRoot[0].SamAccountName;

                viewModel.CreateButtonLabel = Localizer["CreateNewOU"];
                viewModel.EditLinkLabel = Localizer["Rename"];
                viewModel.DataTarget = "#OuCreate";

                return View(viewModel);
            }
            catch (ADException) // Connection à l'AD impossible
            {
                viewModel.ADConnectionError = Localizer["ADConnectionError"];
                return View(viewModel);
            }
        }

        [Authorize]
        public IActionResult UpdateContent(string selectedPathForContent)
        {
            string users = JsonConvert.SerializeObject(ad.getAllUsers().Select(x => x.SamAccountName));
            viewModel.UsersAD = users;
            viewModel.ADManager = ad;


            viewModel.SelectedPath = selectedPathForContent;
            viewModel.ADRoot = ad.getRoot();
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
            var DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
            var split = viewModel.SelectedPath.Split('/');
            var selectedNodeName = split.Length == 2 ? split[1] : split[2];
            // Delete OU, an empty OU has split.Length == 2
            if (split.Length == 2)
            {
                // at this point, the OU does not contain groups since split.Length == 2
                ad.deleteOU(DN);
                Serilog.Log.Information("Le dossier(OU) " + DN + " a été supprimé");
            }
            // Delete Group, a Group has split.Length == 3
            if (split.Length == 3)
            {
                ad.deleteGroup(DN);
                Serilog.Log.Information("Le groupe " + DN + " a été supprimé");
            }
            return RedirectToAction("UpdateContent", "Home", new { selectedPathForContent = viewModel.SelectedPath });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Rename(MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            var DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
            if (ModelState.IsValid)
            {
                ad.renameOU(DN, viewModel.NewName);
                Serilog.Log.Information("Le dossier(OU) " + DN + " a été renommé");
            }

            return RedirectToAction("UpdateContent", "Home", new { selectedPathForContent = viewModel.SelectedPath });
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CreateGroupModal([Bind("GroupAD, SelectedNodeName, SelectedContentName, SelectedPath, SelectedMembers, SelectedOwners")] MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            ApplicationDbContext db = new ApplicationDbContext();


            string[] split = viewModel.SelectedPath.Split('/');
            string selectedNodeName = split.Length == 2 ? split[1] : split[2];
            GroupAD group = viewModel.GroupAD;
            List<UserAD> members = GetSelectedUsersSamAccount(viewModel);
          
           

            ModelState.Remove("NewName");

            if (ModelState.IsValid)
            {

               
                ad.createGroup(selectedNodeName, group, members);

                string DN = FindDN(viewModel.SelectedPath, group.SamAccountName);
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

                return RedirectToAction("MainView", "Home");
            }
            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult EditGroupModal([Bind("GroupAD, SelectedNodeName, SelectedPath, BeforeEditMembers, SelectedMembers, OuGroup, SelectedOwners")] MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            ApplicationDbContext db = new ApplicationDbContext();

            GroupAD group = viewModel.GroupAD;
            string DN = FindDN(viewModel.SelectedPath, viewModel.OuGroup);
            string guid = ad.getGroupGUIDByDn(DN);
            Dictionary<UserAD, Util.Action> updatedGroupMembers = UpdatedGroupMembersKeyValueActions(viewModel);


            List<string> selectedOwnersNames = DeserializeUsers(viewModel.SelectedOwners);
            List<User> selectedOwners = db.User.ToList().Where(x => selectedOwnersNames.Contains(x.GetName())).ToList();
            OwnerGroup ownerGroup = db.OwnerGroup.Where(x => x.GUID == guid).Include(x => x.OwnerGroupUsers).FirstOrDefault();

            ownerGroup.OwnerGroupUsers.Clear();
            selectedOwners.ForEach(user => ownerGroup.OwnerGroupUsers.Add(new OwnerGroupUser { User = user, OwnerGroup = ownerGroup }));

            ModelState.Remove("NewName");
            if (ModelState.IsValid)
            {
                DateTime dateExp = viewModel.GroupAD.ExpirationDate;
                ad.modifyGroup(DN, group, viewModel.SelectedNodeName,  updatedGroupMembers);

                selectedOwners.ForEach(x => db.Entry(x).State = EntityState.Modified);
                db.Entry(ownerGroup).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("MainView");
            }
            return View(viewModel.GroupAD);
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
                ad.createOU(viewModel.NewName);
                Serilog.Log.Information("Le dossier(OU) " + viewModel.NewName + " a été créé");
            }
            return RedirectToAction("UpdateContent", "Home", new { selectedPathForContent = viewModel.SelectedPath });
        }

        private string FindDN(string selectedPath, string selectedContentName)
        {
            viewModel.ADRoot = ad.getRoot();
            return viewModel.ADRoot.Find(e => e.Path == selectedPath && e.SamAccountName == selectedContentName).Dn;
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


        private List<String> DeserializeUsers(string serializedUsers)
        {
            return serializedUsers != null ? JsonConvert.DeserializeObject<List<string>>(serializedUsers) : new();
        }


        public List<UserAD> GetSelectedUsersSamAccount(MainViewViewModel viewModel)
        {
            List<UserAD> members = new();
            if (viewModel.SelectedMembers != null)
            {
                List<string> selectedUsers = JsonConvert.DeserializeObject<List<string>>(viewModel.SelectedMembers);
                members = ad.getAllUsers().Where(x => selectedUsers.Contains(x.SamAccountName)).ToList();
            }
            return members;
        }

        [Authorize]
        public IActionResult GetOwners(string guid)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            ApplicationDbContext db = new ApplicationDbContext();

            var user = db.User.Where(x => x.OwnerGroupUsers.Select(x => x.OwnerGroup.GUID).Contains(guid)).ToList().Select(x => x.GetName());

            viewModel.UsersAD = JsonConvert.SerializeObject(ad.getAllUsers().Select(x => x.SamAccountName));
            viewModel.SelectedOwners = JsonConvert.SerializeObject(user);

            return PartialView("EditOwners", viewModel);
        }
    }

}
