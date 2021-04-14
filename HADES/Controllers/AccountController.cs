using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Models;

namespace HADES.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<IdentityUser> signInManager;

        public AccountController(SignInManager<IdentityUser> signInMngr)
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
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            ModelState.AddModelError("", "Invalid username/password.");
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
            return RedirectToAction("Index", "Home");
        }
    }
}
