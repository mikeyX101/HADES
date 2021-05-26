using HADES.Models;
using HADES.Util;
using HADES.Util.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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

                    Log.Information("{User} logged on from login page", User.GetName());
                    var claims = new List<Claim>{
                            new Claim("id", User.GetId().ToString()),
                            new Claim("isDefault", User.IsDefaultUser().ToString()),
                            new Claim(ClaimTypes.Name, User.GetName(), ClaimValueTypes.String)
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
