using HADES.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Controllers
{
    public class DocumentationController : Controller
    {

        public IActionResult Documentation()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            return View();
        }
    }
}
