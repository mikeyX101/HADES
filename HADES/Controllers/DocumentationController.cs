using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HADES.Controllers
{
	public class DocumentationController : Controller
    {
        [Authorize]
        public IActionResult Documentation()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            return View();
        }
    }
}
