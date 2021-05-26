﻿using HADES.Data;
using HADES.Models;
using HADES.Services;
using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HADES.Controllers
{
	public class UserConfigsController : LocalizedController<HomeController>
    {
        private readonly ApplicationDbContext db;

        public UserConfigsController(IStringLocalizer<HomeController> localizer, ApplicationDbContext context) : base(localizer)
        {
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> UserConfig()
        {
            UserConfigService service = new();
            var viewModel = await service.UserConfig(ConnexionUtil.CurrentUser(this.User).GetUserConfig());
            viewModel.Languages = new List<SelectListItem>() 
            {
                new SelectListItem {Text = HADES.Strings.French, Value = "fr-CA"},
                new SelectListItem {Text = HADES.Strings.English, Value = "en-US"},
                new SelectListItem {Text = HADES.Strings.Spanish, Value = "es-US"},
                new SelectListItem {Text = HADES.Strings.Portuguese, Value = "pt-BR"}
            };
            viewModel.Themes = new List<SelectListItem>()
            {
                new SelectListItem {Text = "site", Value = "site"},
                new SelectListItem {Text = "deeppink", Value = "deeppink"},
                new SelectListItem {Text = "chocolate", Value = "chocolate"},
                new SelectListItem {Text = "greenmint", Value = "greenmint"},
                new SelectListItem {Text = "white", Value = "white"}
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
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
            else
            {
                return View(viewModel);
            }
        }

        [Authorize]
        public IActionResult CreateEmail(int? id)
        {
            ViewBag.UserConfigId = id;
            return View();
        }


        [HttpPost]
        [Authorize]
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


        [Authorize]
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
