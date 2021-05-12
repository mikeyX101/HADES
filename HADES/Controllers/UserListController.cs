using HADES.Data;
using HADES.Models;
using HADES.Services;
using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HADES.Controllers
{
    public class UserListController : Controller
    {
        private readonly ApplicationDbContext db;

        public UserListController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize]
        public IActionResult UserList()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().UserListAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            return View();
        }


    }
}
