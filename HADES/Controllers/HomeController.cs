using HADES.Models;
using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using Newtonsoft.Json;
using HADES.Util.ModelAD;
using HADES.Data;
using System;
using System.Threading.Tasks;
using HADES.Util.Exceptions;
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
        public IActionResult MainView(string selectedPath)
        {

            try
            {
                viewModel.ADRoot = ad.getRoot();
                viewModel.ADManager = ad;
                BuildRootTreeNode(viewModel.ADRoot); // conversion List<RootDataInformation> en TreeNode<string>
                viewModel.ADRootTreeNodeJson = TreeNodeToJson(viewModel.ADRootTreeNode); // conversion TreeNode<string> en Json

                if (selectedPath == null)
                {
                    viewModel.SelectedPath = "/" + viewModel.ADRoot[0].SamAccountName; // select root OU par défaut
                    viewModel.SelectedNodeName = viewModel.ADRoot[0].SamAccountName;
                }
                else
                {
                    var split = selectedPath.Split('/');
                    var selectedNodeName = split.Length == 2 ? split[1] : split[2];
                    viewModel.SelectedPath = selectedPath;
                    viewModel.SelectedNodeName = selectedNodeName;
                }

                viewModel.CreateButtonLabel = Localizer["CreateNewOU"];
                viewModel.EditLinkLabel = Localizer["Rename"];

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

            viewModel.ADRoot = ad.getRoot();
            viewModel.SelectedPath = selectedPathForContent;
            var split = viewModel.SelectedPath.Split('/');
            viewModel.SelectedNodeName = split[split.Length - 1];
            if (split.Length == 2)
            {
                viewModel.CreateButtonLabel = Localizer["CreateNewOU"];
                viewModel.EditLinkLabel = Localizer["Rename"];
                viewModel.DataTarget = "#createOuModal";
            }
            if (split.Length == 3)
            {
                viewModel.CreateButtonLabel = Localizer["CreateNewGroup"];
                viewModel.EditLinkLabel = Localizer["Edit"];
                viewModel.DataTarget = "#GroupCreate";
            }
            return PartialView("_Content", viewModel);
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
            if (split.Length == 2)
            {
                /* TODO : validations dossier ne contient pas de groupes */
                if (true)
                {
                    ad.deleteOU(DN);
                }

            }
            /* TODO : supprimer Group */
            /*if (split.Length == 3)
            {
                ad.deleteGroup(DN);
            }*/
            viewModel.ADRoot = ad.getRoot();
            viewModel.SelectedNodeName = selectedNodeName;
            return RedirectToAction("MainView", "Home", new { selectedPath = viewModel.SelectedPath });
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
            viewModel.ADRoot = ad.getRoot();
            return RedirectToAction("MainView", "Home", new { selectedPath = viewModel.SelectedPath });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroupModal([Bind("GroupAD, SelectedNodeName, SelectedContentName, SelectedPath, SelectedUsers")] MainViewViewModel viewModel)
        {
            //viewModel.SelectedNodeName
            //////////////
            var split = viewModel.SelectedPath.Split('/');
            var selectedNodeName = split.Length == 2 ? split[1] : split[2];
            //////
            var groupAD = viewModel.GroupAD;
            //var sd = viewModel.UsersAD;


            //////////GetSelectedUsersSamAccount()
            List<UserAD> members = new();
            if (viewModel.SelectedUsers != null)
            {
                List<string> selectedUsers = JsonConvert.DeserializeObject<List<string>>(viewModel.SelectedUsers);
                members = ad.getAllUsers().Where(x => selectedUsers.Contains(x.SamAccountName)).ToList();
            }
            ///////

            if (ModelState.IsValid)
            {
                //refactor pour reduire nombre de param, 
                //ad.createGroup(groupAD.SamAccountName, selectedNodeName, groupAD.Description, groupAD.Email, groupAD.Notes, members);

                DateTime dateExp = (DateTime)groupAD.ExpirationDate;
                ad.createGroup(groupAD.SamAccountName, selectedNodeName, groupAD.Description, groupAD.Email, dateExp, groupAD.Notes, groupAD.Members);

                return RedirectToAction("MainView");
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
            ad.createOU(viewModel.NewName);
            viewModel.ADRoot = ad.getRoot();
            return RedirectToAction("MainView", "Home");
        }

        private string FindDN(string selectedPath, string selectedContentName)
        {
            viewModel.ADRoot = ad.getRoot();
            return viewModel.ADRoot.Find(e => e.Path == selectedPath && e.SamAccountName == selectedContentName)?.Dn;
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
    }




}
