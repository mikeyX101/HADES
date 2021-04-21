using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Models;
using Microsoft.Extensions.Localization;
using HADES.Util;
using HADES.Util.Exceptions;

namespace HADES.Controllers
{
    public class AccountController : LocalizedController<AccountController>
    {
        // The ConnexionUtil used by the controller
        private readonly ConnexionUtil connect;

        public AccountController(IStringLocalizer<AccountController> localizer) : base(localizer)
        {
            connect = new ConnexionUtil();

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
                try
                {
                    if (await connect.Login(model.Username, model.Password))
                    {
                        Console.WriteLine(model.Username.ToLower() + " CONNECTED"); // Change this by log
                        return RedirectToAction("MainView", "Home");
                    }
                    else
                    {
                        Console.WriteLine("DEFAULT USER CONNECTED"); // Change this by log
                        return RedirectToAction("MainView", "Home");
                    }

                }
                catch (ForbiddenException)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                catch (LoginException)
                {
                    ModelState.AddModelError("", Localizer["MSG_Invalid"]);
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError("", Localizer["MSG_Invalid"]);
                return View(model);
            }

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
