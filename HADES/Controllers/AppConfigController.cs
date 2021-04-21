using HADES.Data;
using HADES.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Controllers
{
    public class AppConfigController : Controller
    {

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            AppConfigViewModel viewModel = new AppConfigViewModel
            {
                // TODO : get viewModel from DB
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Index(AppConfigViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // TODO : save viewModel in DB
                return RedirectToAction("MainView","Home");
            }
            else
            {
                return View(viewModel);
            }
        }

    }
}
