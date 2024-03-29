﻿using HADES.Data;
using HADES.Models;
using HADES.Services;
using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HADES.Controllers
{
	public class AppConfigController : Controller
    {
        private readonly ApplicationDbContext db;

        public AppConfigController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> AppConfig()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            AppConfigService service = new();

            return View(await service.AppConfigViewModelGET());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AppConfig([Bind("ActiveDirectory,AdminGroups,SuperAdminGroups,DefaultUser,AppConfig")] AppConfigViewModel viewModel, [FromForm] string confirm, [FromForm] string confirmDN, [FromForm] string confirmSMTP, [FromForm] string useSMTPCred, [FromForm] IFormFile bg, [FromForm] IFormFile ico)
        {
            ViewBag.AppConfigError = "";

            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            AppConfigService service = new();

            ValidateModelState();
            if (viewModel.AppConfig.LogDeleteFrequency < 1 || viewModel.AppConfig.LogMaxFileSize < 1) ModelState.AddModelError("LogsInvalid", HADES.Strings.NegativeValueError);

            bool DNencrypted = false;
            if (viewModel.ActiveDirectory.PasswordDN == null || viewModel.ActiveDirectory.PasswordDN.Equals(""))
            {
                // Don't change Password
                confirmDN = viewModel.ActiveDirectory.PasswordDN = service.AppConfigViewModelGET().Result.ActiveDirectory.PasswordDN;
                DNencrypted = true;
            }
            bool SMTPencrypted = false;
            if (useSMTPCred == "on" && (viewModel.AppConfig.SMTPPassword == null || viewModel.AppConfig.SMTPPassword.Equals("")))
            {
                // Don't change Password
                confirmSMTP = viewModel.AppConfig.SMTPPassword = service.AppConfigViewModelGET().Result.AppConfig.SMTPPassword;
                SMTPencrypted = true;
            }

            if (useSMTPCred != "on")
            {
                // Remove all credentials
                viewModel.AppConfig.SMTPUsername = null;
                viewModel.AppConfig.SMTPPassword = null;
            }

            bool hashed = false;
            if (viewModel.DefaultUser.Password == null || viewModel.DefaultUser.Password.Equals(""))
            {
                // Don't change Password
                confirm = viewModel.DefaultUser.Password = service.AppConfigViewModelGET().Result.DefaultUser.Password;
                hashed = true;
            }
            if (bg?.Length > 10485760)
            {
                ModelState.AddModelError("bgimg", HADES.Strings.errorFileSize);
            }
            if (ico?.Length > 10485760)
            {
                ModelState.AddModelError("icoimg", HADES.Strings.errorFileSize);
            }

            if (confirm == null) confirm = "";
            if (confirmDN == null) confirmDN = "";

            if (confirmSMTP == viewModel.AppConfig.SMTPPassword && confirmDN.Equals(viewModel.ActiveDirectory.PasswordDN) && confirm.Equals(viewModel.DefaultUser.Password) && (validatePassword(viewModel.DefaultUser.Password) || hashed) && TryValidateModel(viewModel))
            {
                if (!hashed)
                {
                    viewModel.DefaultUser.Password = ConnexionUtil.HashPassword(viewModel.DefaultUser.Password); // Password is now hashed
                }

                if (!DNencrypted)
                {
                    viewModel.ActiveDirectory.PasswordDN = EncryptionUtil.Encrypt(viewModel.ActiveDirectory.PasswordDN); // Password is now encrypted
                }

                if (!SMTPencrypted && useSMTPCred == "on")
                {
                    viewModel.AppConfig.SMTPPassword = EncryptionUtil.Encrypt(viewModel.AppConfig.SMTPPassword); // Password is now encrypted
                }

                if (ico != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        ico.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        viewModel.AppConfig.CompanyLogoFile = "data:" + ico.ContentType + ";base64," + Convert.ToBase64String(fileBytes);
                    }
                }
                else
                {
                    viewModel.AppConfig.CompanyLogoFile = service.AppConfigViewModelGET().Result.AppConfig.CompanyLogoFile; // don't change
                }

                if (bg != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        bg.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        viewModel.AppConfig.CompanyBackgroundFile = "data:" + bg.ContentType + ";base64," + Convert.ToBase64String(fileBytes);
                    }
                }
                else
                {
                    viewModel.AppConfig.CompanyBackgroundFile = service.AppConfigViewModelGET().Result.AppConfig.CompanyBackgroundFile; // don't change
                }

                // Now Update AppConfig
                try
                {
                    await service.UpdateAppConfig(viewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!service.AppConfigExists(viewModel))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AppConfig));
            }
            ViewBag.AppConfigError = HADES.Strings.ErrorSavingConfiguration;
            return View(viewModel);
        }

        private bool validatePassword(string pass)
        {
            return new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[a-zA-Z\d].{6,}$").IsMatch(pass);
        }

        [Authorize]
        public IActionResult CreateAdminGroup()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            ViewBag.AppConfigId = db.AppConfig.FirstOrDefaultAsync().Result.Id; ;
            return View(new AdminGroup());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminGroup(string DN, int appconfig)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }
            try
            {
                AppConfigService service = new();
                string GroupGUID = new ADManager().getGroupGUIDByDn(DN);
                if (GroupGUID != "")
                {
                    try
                    {
                        await service.AddAdminGroup(new AdminGroup() { GUID = GroupGUID, AppConfigId = appconfig });
                    }
                    catch (Exception) { }

                    return RedirectToAction("AppConfig");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("AppConfig");
            }
            ViewBag.Error = HADES.Strings.GenericError;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> AdminGroupDelete(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (id == null)
            {
                return NotFound();
            }
            await service.DeleteAdminGroup(id);

            return RedirectToAction("AppConfig");
        }

        [Authorize]
        public IActionResult CreateSuperAdminGroup()
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            ViewBag.AppConfigId = db.AppConfig.FirstOrDefaultAsync().Result.Id;
            return View(new SuperAdminGroup());
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSuperAdminGroup(string DN, int appconfig)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            try
            {
                AppConfigService service = new();
                string GroupGUID = new ADManager().getGroupGUIDByDn(DN);

                if (GroupGUID != "")
                {
                    try
                    {
                        await service.AddSuperAdminGroup(new SuperAdminGroup() { GUID = GroupGUID, AppConfigId = appconfig });
                    }
                    catch (Exception) { }

                    return RedirectToAction("AppConfig");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("AppConfig");
            }
            ViewBag.Error = HADES.Strings.GenericError;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> SuperAdminGroupDelete(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (id == null)
            {
                return NotFound();
            }
            var appConfigId = service.SuperAdminGroupRedirectId(id);
            await service.DeleteSuperAdminGroup(id);

            return RedirectToAction("AppConfig", new { id = appConfigId });
        }


        public void ValidateModelState()
        {
            ModelState.Remove("ActiveDirectory.Id");
            ModelState.Remove("DefaultUser.Id");
            ModelState.Remove("DefaultUser.UserConfigId");
            ModelState.Remove("DefaultUser.RoleId");
            ModelState.Remove("AppConfig.Id");
            ModelState.Remove("DefaultUser.Password");
            ModelState.Remove("ActiveDirectory.PasswordDN");
        }

        
    }
}
