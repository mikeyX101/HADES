using HADES.Data;
using HADES.Models;
using Microsoft.AspNetCore.Authorization;
using HADES.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public async Task<IActionResult> AppConfig()
        {
            AppConfigService service = new();

            return View(await service.AppConfigViewModelGET());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppConfig([Bind("ActiveDirectory,AdminGroups,SuperAdminGroups,DefaultUser,AppConfig")] AppConfigViewModel viewModel)
        {
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

        public IActionResult CreateAdminGroup(int? id)
        {
            ViewBag.AppConfigId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminGroup([Bind("Id,SamAccount,AppConfigId")] AdminGroup adminGroup)
        {
            AppConfigService service = new();
            if (ModelState.IsValid)
            {
                await service.AddAdminGroup(adminGroup);
                return RedirectToAction("AppConfig", new { id = adminGroup.AppConfigId });
            }
            return View(adminGroup);
        }

        public async Task<IActionResult> AdminGroupDelete(int? id)
        {
            AppConfigService service = new();
            if (id == null)
            {
                return NotFound();
            }
            var appConfigId = service.AdminGroupRedirectId(id);
            await service.DeleteAdminGroup(id);

            return RedirectToAction("AppConfig", new { id = appConfigId });
        }

        public IActionResult CreateSuperAdminGroup(int? id)
        {
            ViewBag.AppConfigId = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSuperAdminGroup([Bind("Id,SamAccount,AppConfigId")] SuperAdminGroup superAdminGroup)
        {
            AppConfigService service = new();
            if (ModelState.IsValid)
            {
                await service.AddSuperAdminGroup(superAdminGroup);
                return RedirectToAction("AppConfig", new { id = superAdminGroup.AppConfigId });
            }
            return View(superAdminGroup);
        }

        public async Task<IActionResult> SuperAdminGroupDelete(int? id)
        {
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
