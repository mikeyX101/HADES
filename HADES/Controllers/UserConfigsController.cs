using HADES.Models;
using HADES.Services;
using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Controllers
{
	public class UserConfigsController : LocalizedController<HomeController>
    {

        public UserConfigsController(IStringLocalizer<HomeController> localizer) : base(localizer) { }

        [Authorize]
        public IActionResult UserConfig()
        {
            UserConfigService service = new();
            var viewModel = service.UserConfig(ConnexionUtil.CurrentUser(User).GetUserConfig());
            viewModel.Languages = new List<SelectListItem>()
            {
                new SelectListItem {Text = Strings.French, Value = "fr-CA"},
                new SelectListItem {Text = Strings.English, Value = "en-US"},
               // new SelectListItem {Text = HADES.Strings.Spanish, Value = "es-US"}, // SUPPORT es-US
               // new SelectListItem {Text = HADES.Strings.Portuguese, Value = "pt-BR"} // SUPPORT pt-BR
            };
            viewModel.Themes = new List<SelectListItem>()
            {
                new SelectListItem {Text = Strings.Dark, Value = "site"},
                new SelectListItem {Text = Strings.Green, Value = "greenmint"},
                new SelectListItem {Text = Strings.Light, Value = "white"}
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserConfig([Bind("UserConfig,Emails")] UserConfigViewModel viewModel)
        {
            UserConfigService service = new();
            if (viewModel.Emails != null && !AreEmailAddressesUnique(viewModel))
            {
                ModelState.AddModelError("", HADES.Strings.EmailsNotUnique);
            }
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

        private bool AreEmailAddressesUnique(UserConfigViewModel viewModel)
        {
            return viewModel.Emails.Select(item => item.Address).Distinct().Count() == viewModel.Emails.Select(item => item.Address).Count();
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
        public async Task<IActionResult> EmailDelete(string id)
        {
            UserConfigService service = new();
            if (int.TryParse(id, out int emailId))
			{
                await service.DeleteEmail(emailId);
            }
            

            return RedirectToAction("UserConfig");
        }
    }
}
