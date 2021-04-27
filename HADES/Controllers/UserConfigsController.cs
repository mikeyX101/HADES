using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HADES.Data;
using HADES.Models;
using HADES.Services;

namespace HADES.Controllers
{
    public class UserConfigsController : Controller
    {
        private readonly ApplicationDbContext db;

        public UserConfigsController(ApplicationDbContext context)
        {
            db = context;
        }


        public async Task<IActionResult> UserConfig(int? id)
        {
            UserConfigService service = new();
            var viewModel = await service.UserConfig(id);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserConfig([Bind("UserConfig,Emails")] UserConfigViewModel viewModel)
        {
            UserConfigService service = new();
            if (ModelState.IsValid)
            {
                try
                {
                    await service.UpdateUserConfig(viewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!service.UserConfigExists(viewModel.UserConfig))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("UserConfig", new { id = viewModel.UserConfig.Id });
            }

            return View(viewModel);
        }

        public IActionResult CreateEmail(int? id)
        {
            ViewBag.UserConfigId = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmail([Bind("Id,Address,ExpirationDate,GroupCreate,GroupDelete,MemberAdd,MemberRemoval,UserConfigId")] Email email)
        {
            UserConfigService service = new();
            if (ModelState.IsValid)
            {
                await service.AddEmail(email);
                return RedirectToAction("UserConfig", new { id = email.UserConfigId });
            }
            return View(email);
        }


        public async Task<IActionResult> EmailDelete(int? id)
        {
            UserConfigService service = new();
            if (id == null)
            {
                return NotFound();
            }
            var userConfigId = await service.RedirectId(id);
            await service.DeleteEmail(id);

            return RedirectToAction("UserConfig", new { id = userConfigId });
        }
    }
}
