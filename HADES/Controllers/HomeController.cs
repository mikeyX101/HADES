using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using HADES.Util;
using HADES.Models;
using System.Diagnostics;

namespace HADES.Controllers
{
    public class HomeController : LocalizedController<HomeController>
    {
        public HomeController(IStringLocalizer<HomeController> localizer) : base(localizer)
        {
        }

        public IActionResult Login()
        {
            return View();
        }

        // Returns the Main Application View parameter is the selected Folder
        public IActionResult MainView(/*Folder f*/)
        {
            // Fill ViewBag with Folders and Groups to display as a TreeSet
            var adManager = new ADManager();
            var root = adManager.getRoot();
           
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
