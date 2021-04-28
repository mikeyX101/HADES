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
        public IActionResult Login()
        {
            return View();
        }

        // Returns the Main Application View parameter is the selected Folder
        public IActionResult MainView()
        {
            try
            {
                viewModel.ADRoot = ad.getRoot();
                BuildRootTreeNode(viewModel.ADRoot);
                viewModel.SelectedPath = "/" + viewModel.ADRoot[0].SamAccountName;
                viewModel.ADRootTreeNodeJson = TreeNodeToJson(viewModel.ADRootTreeNode);

                ViewBag.UserName = ConnexionUtil.CurrentUser(User).GetName();
                ViewBag.UserRole = ConnexionUtil.CurrentUser(User).GetRole().Name;
                //ViewBag.CompanyName = context.AppConfig.Find(1).CompanyName;

                return View(viewModel);
            }
            catch (ADException ex)
            {
                viewModel.ADConnectionError = Localizer["ADConnectionError"];
                return View("Error", viewModel);
            }
        }

        public IActionResult UpdateContent(string id)
        {
            viewModel.ADRoot = ad.getRoot();
            viewModel.SelectedPath = id;
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

        public IActionResult CreateGroupModal()
        {
            return PartialView();
        }


    }
}
