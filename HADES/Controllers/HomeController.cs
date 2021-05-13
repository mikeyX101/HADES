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
                viewModel.ADRoot = ad.getRoot();
                BuildRootTreeNode(viewModel.ADRoot); // conversion List<RootDataInformation> en TreeNode<string>
                viewModel.ADRootTreeNodeJson = TreeNodeToJson(viewModel.ADRootTreeNode); // conversion TreeNode<string> en Json
                viewModel.SelectedPath = "/" + viewModel.ADRoot[0].SamAccountName; // select root OU par d�faut
                viewModel.SelectedNodeName = viewModel.ADRoot[0].SamAccountName;

                viewModel.CreateButtonLabel = Localizer["CreateNewOU"];
                viewModel.EditLinkLabel = Localizer["Rename"];

                return View(viewModel);
            }
            catch (ADException) // Connection � l'AD impossible
            {
                viewModel.ADConnectionError = Localizer["ADConnectionError"];
                return View(viewModel);
            }
        }

        [Authorize]
        public IActionResult UpdateContent(string selectedPathForContent)
        {
            viewModel.SelectedPath = selectedPathForContent;
            viewModel.ADRoot = ad.getRoot();
            BuildRootTreeNode(viewModel.ADRoot); // conversion List<RootDataInformation> en TreeNode<string>
            viewModel.ADRootTreeNodeJson = TreeNodeToJson(viewModel.ADRootTreeNode); // conversion TreeNode<string> en Json
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

                if (viewModel.ADRootTreeNode == null) {
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
                return RedirectToAction("MainView","Home");
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
            Console.WriteLine("L'OU " + DN + " a �t� supprim�");
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
            Console.WriteLine("L'OU " + DN + " a �t� renomm�");
            return RedirectToAction("UpdateContent", "Home", new { selectedPathForContent = viewModel.SelectedPath });
        }

        [Authorize]
        public IActionResult CreateGroupModal()
        {
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroupModal([Bind("GroupAD, SelectedNodeName, SelectedContentName, SelectedPath")] MainViewViewModel viewModel)
        {
            var split = viewModel.SelectedPath.Split('/');
            var selectedNodeName = split.Length == 2 ? split[1] : split[2];

            var groupAD = viewModel.GroupAD;
            if (ModelState.IsValid)
            {
                ad.createGroup(groupAD.SamAccountName, selectedNodeName, groupAD.Description, groupAD.Email, groupAD.Notes, groupAD.Members);
                //return RedirectToAction("EditGroupModal");
                return RedirectToAction("MainView");
            }
            return View(groupAD);
        }

        [Authorize]
        public IActionResult EditGroupModal()
        {
            var DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);

            var group = ad.getGroupInformation(DN);

            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroupModal([Bind("GroupAD, SelectedNodeName, SelectedContentName, SelectedPath")] MainViewViewModel viewModel)
        {
            //var DN = FindDN(viewModel.SelectedPath, viewModel.SelectedContentName);
            if (ModelState.IsValid)
            {
                try
                {
                    //ad.modifyGroup(DN);
                }
                catch (Exception e)
                {

                }
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
            Console.WriteLine("L'OU " + viewModel.NewName + " a �t� cr��");
            return RedirectToAction("UpdateContent", "Home", new { selectedPathForContent = viewModel.SelectedPath });
        }

        private string FindDN(string selectedPath, string selectedContentName)
        {
            viewModel.ADRoot = ad.getRoot();
            return viewModel.ADRoot.Find(e => e.Path == selectedPath && e.SamAccountName == selectedContentName).Dn;
        }
    }




}
