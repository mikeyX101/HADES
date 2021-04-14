using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Models;
using Microsoft.Extensions.Localization;

namespace HADES.Controllers
{
    public class AccountController : LocalizedController<AccountController>
    {
        private SignInManager<IdentityUser> signInManager;

        public AccountController(SignInManager<IdentityUser> signInMngr, IStringLocalizer<AccountController> localizer) : base(localizer)
        {
            signInManager = signInMngr;
        }

        [HttpGet]
        public IActionResult LogIn(string returnURL = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnURL };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // PasswordSignInAsync() logs in a user and returns an IdentityResult object.
                // When lockoutOnFailure is set to true, Identity locks the user out if the sign in fails
                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password, isPersistent: false,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Home", "MainView");
                }
                else
                {
                    return RedirectToAction("LogIn", "Account");
                }
            }
            ModelState.AddModelError("", Localizer["MSG_Invalid"]);
            return View(model);
        }

        public ViewResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("LogIn", "Account");
        }
    }
}
