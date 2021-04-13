﻿using HADES.Models;
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

        }

        // Redirects the User to either the root OU or the Login page depending on the session state.
        public IActionResult Redirect()
        {
            if (true)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("MainView");
            }

        }

        public IActionResult Login()
        {
            return View();
        }

        // Returns the Main Application View parameter is the selected Folder
        public IActionResult MainView(/*Folder f*/)
        {
            return View();
        }

    }
}
