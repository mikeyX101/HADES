using HADES.Data;
using HADES.Models;
using HADES.Services;
using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HADES.Controllers
{
	public class UserConfigsController : LocalizedController<HomeController>
    {

        public UserConfigsController(IStringLocalizer<HomeController> localizer, ApplicationDbContext context) : base(localizer)
        {
        }

        [Authorize]
        public IActionResult UserConfig()
        {
            UserConfigService service = new();
            var viewModel = service.UserConfig(ConnexionUtil.CurrentUser(this.User).GetUserConfig());
            viewModel.Languages = new List<SelectListItem>()
            {
                new SelectListItem {Text = HADES.Strings.French, Value = "fr-CA"},
                new SelectListItem {Text = HADES.Strings.English, Value = "en-US"},
               // new SelectListItem {Text = HADES.Strings.Spanish, Value = "es-US"}, // SUPPORT es-US
               // new SelectListItem {Text = HADES.Strings.Portuguese, Value = "pt-BR"} // SUPPORT pt-BR
            };
            viewModel.Themes = new List<SelectListItem>() //TODO Translate text with readable and not "technical" names
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
                return RedirectToAction("UserConfig");
            }
            else
            {
                return View(viewModel);
            }
        }

        [Authorize]
        public IActionResult CreateEmail()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmail([Bind("Id,Address,ExpirationDate,GroupCreate,GroupDelete,MemberAdd,MemberRemoval")] Email email)
        {
            UserConfigService service = new();
            if (ModelState.IsValid)
            {
                email.UserConfigId = ConnexionUtil.CurrentUser(this.User).GetUserConfig().Id;
                await service.AddEmail(email);
                return RedirectToAction("UserConfig");
            }
            return View(email);
        }


        [Authorize]
        public async Task<IActionResult> EmailDelete()
        {
            UserConfigService service = new();
            int id = ConnexionUtil.CurrentUser(this.User).GetUserConfig().Id;
            await service.DeleteEmail(id);

            return RedirectToAction("UserConfig");
        }
    }
}
