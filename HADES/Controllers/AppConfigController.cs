using HADES.Data;
using HADES.Models;
using HADES.Services;
using HADES.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
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
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            AppConfigService service = new();

            return View(await service.AppConfigViewModelGET());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AppConfig([Bind("ActiveDirectory,AdminGroups,SuperAdminGroups,DefaultUser,AppConfig")] AppConfigViewModel viewModel, string confirm, string confirmDN, string confirmSMTP, string useSMTPCred)
        {
            ViewBag.AppConfigError = "";

            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            AppConfigService service = new();

            ValidateModelState();

            if (viewModel.ActiveDirectory.PasswordDN == null || viewModel.ActiveDirectory.PasswordDN.Equals(""))
            {
                // Don't change Password
                confirmDN = viewModel.ActiveDirectory.PasswordDN = service.AppConfigViewModelGET().Result.ActiveDirectory.PasswordDN;
            }

            if (useSMTPCred == "on" && (viewModel.AppConfig.SMTPPassword == null || viewModel.AppConfig.SMTPPassword.Equals("")))
            {
                // Don't change Password
                confirmSMTP = viewModel.AppConfig.SMTPPassword = service.AppConfigViewModelGET().Result.AppConfig.SMTPPassword;
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

            if (confirm == null) confirm = "";
            if (confirmDN == null) confirmDN = "";

            if (confirmDN.Equals(viewModel.ActiveDirectory.PasswordDN) && confirm.Equals(viewModel.DefaultUser.Password) && (validatePassword(viewModel.DefaultUser.Password) || hashed) && TryValidateModel(viewModel))
            {
                if (!hashed)
                {
                    viewModel.DefaultUser.Password = ConnexionUtil.HashPassword(viewModel.DefaultUser.Password); // Password is now hashed
                }

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
        public IActionResult CreateAdminGroup(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }
            ViewBag.AppConfigId = id ??= db.AppConfig.FirstOrDefaultAsync().Result.Id; ;
            return View(new AdminGroup());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminGroup([Bind("Id,SamAccount,AppConfigId")] AdminGroup adminGroup)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (ModelState.IsValid)
            {
                await service.AddAdminGroup(adminGroup);
                return RedirectToAction("AppConfig", new { id = adminGroup.AppConfigId });
            }
            return View(adminGroup);
        }

        [Authorize]
        public async Task<IActionResult> AdminGroupDelete(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (id == null)
            {
                return NotFound();
            }
            var appConfigId = service.AdminGroupRedirectId(id);
            await service.DeleteAdminGroup(id);

            return RedirectToAction("AppConfig", new { id = appConfigId });
        }

        [Authorize]
        public IActionResult CreateSuperAdminGroup(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            ViewBag.AppConfigId = id ??= db.AppConfig.FirstOrDefaultAsync().Result.Id;
            return View(new SuperAdminGroup());
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSuperAdminGroup([Bind("Id,SamAccount,AppConfigId")] SuperAdminGroup superAdminGroup)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
            {
                return RedirectToAction("MainView", "Home");
            }

            AppConfigService service = new();
            if (ModelState.IsValid)
            {
                await service.AddSuperAdminGroup(superAdminGroup);
                return RedirectToAction("AppConfig", new { id = superAdminGroup.AppConfigId });
            }
            return View(superAdminGroup);
        }

        [Authorize]
        public async Task<IActionResult> SuperAdminGroupDelete(int? id)
        {
            if (!ConnexionUtil.CurrentUser(this.User).GetRole().AppConfigAccess)
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


        public static string Encrypt(string pass)
        {
            DES des = DES.Create();
            des.GenerateKey();
            return null;
        }

        public static string Decrypt(string pass)
        {
            return null;
        }
    }
}
