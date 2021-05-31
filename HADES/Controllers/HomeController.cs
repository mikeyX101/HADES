using HADES.Data;
using HADES.Models;
using HADES.Util;
using HADES.Util.Exceptions;
using HADES.Util.ModelAD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private ApplicationDbContext context;

        public HomeController(IStringLocalizer<HomeController> localizer, ApplicationDbContext ctx) : base(localizer)
        {
            ad = new ADManager();
            viewModel = new MainViewViewModel();
            context = ctx;
        }

        [Authorize]
        // Returns the Main Application View parameter is the selected Folder
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
                viewModel.ExpandedNodesName = JsonConvert.SerializeObject(new string[] { viewModel.ADRoot[0].SamAccountName }, Formatting.Indented);

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

        [HttpPost]
        [HttpGet]
        [Authorize]
        public IActionResult UpdateContent(string selectedPathForContent, string expandedNodeNames)
        {
            string users = JsonConvert.SerializeObject(ad.getAllUsers().Select(x => x.SamAccountName));
            viewModel.UsersAD = users;
            viewModel.ADManager = ad;

            viewModel.SelectedPath = selectedPathForContent;
            viewModel.ExpandedNodesName = expandedNodeNames;
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
            return RedirectToAction("UpdateContent", "Home", new { 
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
            var DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
            if (ModelState.IsValid)
            {
                ad.renameOU(DN, viewModel.NewName);
                Serilog.Log.Information("Le dossier(OU) " + DN + " a été renommé");
            }

            return RedirectToAction("UpdateContent", "Home", new { 
                selectedPathForContent = viewModel.SelectedPath, 
                expandedNodeNames = viewModel.ExpandedNodesName
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateGroupModal([Bind("GroupAD, SelectedNodeName, SelectedContentName, SelectedPath, SelectedUsers")] MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            string[] split = viewModel.SelectedPath.Split('/');
            string selectedNodeName = split.Length == 2 ? split[1] : split[2];
            GroupAD group = viewModel.GroupAD;
            List<UserAD> members = GetSelectedUsersSamAccount(viewModel);

            ModelState.Remove("NewName");

            if (ModelState.IsValid)
            {
                //datepicker sam
                //ajouter owner
                DateTime dateExp = DateTime.Now;
                ad.createGroup(selectedNodeName, group, dateExp, members);

                return RedirectToAction("MainView", "Home");
            }
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGroupModal([Bind("GroupAD, SelectedNodeName, SelectedPath, BeforeEditMembers, SelectedMembers, OuGroup")] MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            GroupAD group = viewModel.GroupAD;
            string DN = FindDN(viewModel.SelectedPath, viewModel.OuGroup);

            Dictionary<UserAD, Util.Action> updatedGroupMembers = UpdatedGroupMembersKeyValueActions(viewModel);

            ModelState.Remove("NewName");
            if (ModelState.IsValid)
            {
                //datepicker sam
                //edit owner
                DateTime dateExp = DateTime.Now;
                ad.modifyGroup(DN, group, viewModel.SelectedNodeName, dateExp, updatedGroupMembers);

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
            return RedirectToAction("UpdateContent", "Home", new {
                selectedPathForContent = viewModel.SelectedPath,
                expandedNodeNames = viewModel.ExpandedNodesName });
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
    }

}
