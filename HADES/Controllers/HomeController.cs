using HADES.Models;
using HADES.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HADES.Controllers
{
    public class HomeController : LocalizedController<HomeController>
    {
        private ADManager ad;

        public HomeController(IStringLocalizer<HomeController> localizer) : base(localizer)
        {
            ad = new ADManager();
        }

        public IActionResult Login()
        {
            return View();
        }

        // Returns the Main Application View parameter is the selected Folder
        public IActionResult MainView(/*Folder f*/)
        {
            // Fill ViewBag with Folders and Groups to display as a TreeSet
            var adRoot = ad.getRoot();

            TreeNode<string> root = new TreeNode<string>("root");
            {
                TreeNode<string> node0 = root.AddChild("node0");
                TreeNode<string> node1 = root.AddChild("node1");
                TreeNode<string> node2 = root.AddChild("node2");
                {
                    TreeNode<string> node20 = node2.AddChild("node20");
                    TreeNode<string> node21 = node2.AddChild("node21");
                    {
                        TreeNode<string> node210 = node21.AddChild("node210");
                        TreeNode<string> node211 = node21.AddChild("node211");
                    }
                }
                TreeNode<string> node3 = root.AddChild("node3");
                {
                    TreeNode<string> node30 = node3.AddChild("node30");
                }
            }
            var rootJson = JsonConvert.SerializeObject(root, Formatting.Indented,
                                            new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore,
                                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                            });
            rootJson = rootJson.Replace("\"nodes\": []", "");
            ViewBag.output = rootJson;
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
