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
using System.Threading.Tasks;
using System.Linq;
using HADES.Models.API;

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
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            var DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
            var split = viewModel.SelectedPath.Split('/');
            var selectedNodeName = split.Length == 2 ? split[1] : split[2];
            // Delete OU
            if (split.Length == 2)
            {
                /* TODO : validations dossier ne contient pas de groupes */
                if (true)
                {
                    ad.deleteOU(DN);
                }

            }
            // Delete Group
            if (split.Length == 3)
            {
                /* TODO : validations group */
                if (true)
                {
                    ad.deleteGroup(DN);
                }

            }
            Serilog.Log.Information("Le dossier(OU) " + DN + " a été supprimé");
            return RedirectToAction("UpdateContent", "Home", new { selectedPathForContent = viewModel.SelectedPath });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Rename(MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            var DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
            ad.renameOU(DN, viewModel.NewName);
            Serilog.Log.Information("Le dossier(OU) " + DN + " a été renommé");
            return RedirectToAction("UpdateContent", "Home", new { selectedPathForContent = viewModel.SelectedPath });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroupModal([Bind("GroupAD, SelectedNodeName, SelectedContentName, SelectedPath, SelectedUsers")] MainViewViewModel viewModel)
        {
            string[] split = viewModel.SelectedPath.Split('/');
            string selectedNodeName = split.Length == 2 ? split[1] : split[2];
            GroupAD group = viewModel.GroupAD;
            List<UserAD> members = GetSelectedUsersSamAccount(viewModel);

            ModelState.Remove("NewName");

            if (ModelState.IsValid)
            {
                //ajouter datepicker
                DateTime dateExp = DateTime.Now;
                ad.createGroup(selectedNodeName, group, dateExp, members);

                return RedirectToAction("MainView", "Home");
            }
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //ancien samaccountname pour le rename admanager
        public async Task<IActionResult> EditGroupModal([Bind("GroupAD, SelectedNodeName, SelectedPath, BeforeEditMembers, SelectedUsers, OuGroup")] MainViewViewModel viewModel)
        {
            GroupAD group = viewModel.GroupAD;
            string DN = FindDN(viewModel.SelectedPath, viewModel.OuGroup);
            string selectedNodeName = viewModel.SelectedNodeName;

            Dictionary<UserAD, Util.Action> updatedGroupMembers = UpdatedGroupMembersKeyValueActions(viewModel);


            if (ModelState.IsValid)
            {
                //ad.modifyGroup(DN, group.SamAccountName, selectedNodeName, group.Description, group.Email, group.Notes, updatedGroupMembers);

                //changer signature de la methode à vero dans admanager. GARDER OUGROUP (3e param)
                //public bool modifyGroup(string dnGroupToModify, GroupAD group, Dictionary<UserAD, Action> members)

                return RedirectToAction("MainView");
            }
            return View(viewModel.GroupAD);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateOU(MainViewViewModel viewModel)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AdCrudAccess)
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
            //deserialize?
            ////////////////////////
            List<UserAD> initialUsers = new();
            List<UserAD> modifiedUsers = new();

            List<string> selectedUsers = new();
            List<string> beforeEditUsers = new();

            if (viewModel.SelectedUsers != null)
                selectedUsers = JsonConvert.DeserializeObject<List<string>>(viewModel.SelectedUsers);
            if (viewModel.BeforeEditMembers != null)
                beforeEditUsers = JsonConvert.DeserializeObject<List<string>>(viewModel.BeforeEditMembers);
            //////////////////////////////////


            modifiedUsers = ad.getAllUsers().Where(x => selectedUsers.Contains(x.SamAccountName)).ToList();
            initialUsers = ad.getAllUsers().Where(x => beforeEditUsers.Contains(x.SamAccountName)).ToList();

            Dictionary<UserAD, Util.Action> updatedKeyValueActions = new();

            foreach (var member in modifiedUsers)
            {
                if (!initialUsers.Contains(member))
                    updatedKeyValueActions.Add(member, Util.Action.ADD);
            }

            foreach (var member in initialUsers)
            {
                if (!modifiedUsers.Contains(member))
                    updatedKeyValueActions.Add(member, Util.Action.DELETE);
            }

            return updatedKeyValueActions;
        }

        public List<UserAD> GetSelectedUsersSamAccount(MainViewViewModel viewModel)
        {
            List<UserAD> members = new();
            if (viewModel.SelectedUsers != null)
            {
                List<string> selectedUsers = JsonConvert.DeserializeObject<List<string>>(viewModel.SelectedUsers);
                members = ad.getAllUsers().Where(x => selectedUsers.Contains(x.SamAccountName)).ToList();
            }
            return members;
        }
    }

}
