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
using Novell.Directory.Ldap;
using HADES.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace HADES.Controllers
{
    public class AccountController : LocalizedController<AccountController>
    {

        public AccountController(IStringLocalizer<AccountController> localizer) : base(localizer)
        {

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MainView", "Home");
            }
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MainView", "Home");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    IUser User = ConnexionUtil.Login(model.Username, model.Password);
                    if (!User.IsDefaultUser())
                    {
                        Console.WriteLine(User.GetName() + " CONNECTED"); // Change this by log
                        return RedirectToAction("MainView", "Home");
                    }
                    else
                    {
                        Console.WriteLine("DEFAULT USER " + User.GetName() + " CONNECTED"); // Change this by log

                        var claims = new List<Claim>{
                                new Claim("id", User.GetId().ToString()),
                                new Claim("isDefault", User.IsDefaultUser().ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(
                          claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties();

                        await HttpContext.SignInAsync(
                          CookieAuthenticationDefaults.AuthenticationScheme,
                          new ClaimsPrincipal(claimsIdentity),
                          authProperties);
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
                catch (ADException)
                {
                    ModelState.AddModelError("", Localizer["MSG_LDAP"]);
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError("", Localizer["MSG_Invalid"]);
                return View(model);
            }

        }

        [AllowAnonymous]
        public ViewResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LogIn", "Account");
        }
    }
}
