using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Models;
using Microsoft.Extensions.Localization;
using HADES.Util;

namespace HADES.Controllers
{
    public class AccountController : LocalizedController<AccountController>
    {
        // The ConnexionUtil used by the controller
        private readonly ConnexionUtil connect;

        public AccountController(IStringLocalizer<AccountController> localizer) : base(localizer)
        {
            connect = new ConnexionUtil();

            ADManager ad = new ADManager();

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
                

                if (true)
                {
                    return RedirectToAction("MainView", "Home");
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
            //await signInManager.SignOutAsync();
            return RedirectToAction("LogIn", "Account");
        }
    }
}
