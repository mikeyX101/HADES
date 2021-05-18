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
	public class AppConfigController : Controller
    {
        private readonly ApplicationDbContext db;

        public AppConfigController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> AppConfig()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            AppConfigService service = new();

            return View(await service.AppConfigViewModelGET());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AppConfig([Bind("ActiveDirectory,AdminGroups,SuperAdminGroups,DefaultUser,AppConfig")] AppConfigViewModel viewModel)
        {

            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            AppConfigService service = new();

            ValidateModelState();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.UpdateAppConfig(viewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!service.AppConfigExists(viewModel))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AppConfig));
            }
            return View(viewModel);
        }

        [Authorize]
        public IActionResult CreateAdminGroup(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            ViewBag.AppConfigId = id ??= db.AppConfig.FirstOrDefaultAsync().Result.Id; ;
            return View(new AdminGroup());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminGroup([Bind("Id,SamAccount,AppConfigId")] AdminGroup adminGroup)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (ModelState.IsValid)
            {
                await service.AddAdminGroup(adminGroup);
                return RedirectToAction("AppConfig", new { id = adminGroup.AppConfigId });
            }
            return View(adminGroup);
        }

        [Authorize]
        public async Task<IActionResult> AdminGroupDelete(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (id == null)
            {
                return NotFound();
            }
            var appConfigId = service.AdminGroupRedirectId(id);
            await service.DeleteAdminGroup(id);

            return RedirectToAction("AppConfig", new { id = appConfigId });
        }

        [Authorize]
        public IActionResult CreateSuperAdminGroup(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            ViewBag.AppConfigId = id??=db.AppConfig.FirstOrDefaultAsync().Result.Id;
            return View(new SuperAdminGroup());
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSuperAdminGroup([Bind("Id,SamAccount,AppConfigId")] SuperAdminGroup superAdminGroup)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (ModelState.IsValid)
            {
                await service.AddSuperAdminGroup(superAdminGroup);
                return RedirectToAction("AppConfig", new { id = superAdminGroup.AppConfigId });
            }
            return View(superAdminGroup);
        }

        [Authorize]
        public async Task<IActionResult> SuperAdminGroupDelete(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (id == null)
            {
                return NotFound();
            }
            var appConfigId = service.SuperAdminGroupRedirectId(id);
            await service.DeleteSuperAdminGroup(id);

            return RedirectToAction("AppConfig", new { id = appConfigId });
        }


        public void ValidateModelState()
        {
            ModelState.Remove("ActiveDirectory.Id");
            ModelState.Remove("DefaultUser.Id");
            ModelState.Remove("DefaultUser.UserConfigId");
            ModelState.Remove("DefaultUser.RoleId");
            ModelState.Remove("AppConfig.Id");
        }

    }
}
