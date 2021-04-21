﻿using HADES.Models;
using HADES.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Controllers
{
    public class HomeController : LocalizedController<HomeController>
    {
        public HomeController(IStringLocalizer<HomeController> localizer) : base(localizer)
        {
            ADManager ad = new ADManager();
        }

        public IActionResult Login()
        {
            return View();
        }

        // Returns the Main Application View parameter is the selected Folder
        public IActionResult MainView(/*Folder f*/)
        {
            // Fill ViewBag with Folders and Groups to display as a TreeSet
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
