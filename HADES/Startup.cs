using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace HADES
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Use settings to access appsettings.json 
            Settings.Initiate(configuration);
        }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Should be before anything else
			#region Reverse Proxy Setup
			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardLimit = 1;
				options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

				options.KnownNetworks.Clear();
				options.KnownProxies.Clear();
				options.KnownProxies.Add(System.Net.IPAddress.Loopback);

			});
            #endregion

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMemoryCache();
            services.AddSession();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "HADES_AUTH";
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = new TimeSpan(12, 0, 0);
                options.Cookie.HttpOnly = true;
            });

            services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseSqlite(Settings.AppSettings.SqlLiteConnectionString));

            services.AddControllersWithViews();
            services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));

            #region Localization Setup
            // Configure localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                // Add supported cultures here
                IList<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("fr-CA"),
                    new CultureInfo("en-US"),
                    new CultureInfo("es-US"),
                    new CultureInfo("pt-BR")
                };
                // Set default culture to fr (ServiceCulture and UICulture)
                options.DefaultRequestCulture = new RequestCulture(Settings.AppSettings.DefaultCulture, Settings.AppSettings.DefaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                /*
					Culture Providers are used to know which culture to use.
					If a provider returns null, the next provider in the list is used until a non-null result is returned.

					Default order:
						Accepted-Language header provider
						Query string provider (ex: <url>?l=fr-CA)
						Cookie provider

					We should clear the default providers and only use our own.
				*/
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new HadesCultureProvider());
            });

            // Set resource path for localizations
            services.AddLocalization(localOptions => { localOptions.ResourcesPath = Settings.AppSettings.LocalResourcesPath; });

            services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.SubFolder);
            services.AddMvc().AddDataAnnotationsLocalization();
            #endregion
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{

			if (env.IsDevelopment())
			{
				app.UseHttpsRedirection();
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
			}
			else if (env.IsProduction())
			{
				// UseForwardedHeaders() must be executed before UseHtst().
				app.UseForwardedHeaders();
				app.UseExceptionHandler("/Home/Error");
                app.UseHttpsRedirection();
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();

				// If production build, run migrations to keep database up-to-date.
				using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					scope.ServiceProvider.GetRequiredService<Data.ApplicationDbContext>().Database.Migrate();
				}
			}

			app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict, Secure = CookieSecurePolicy.Always });

            app.UseSession();

            // Apply localization service to app
            IOptions<RequestLocalizationOptions> options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=AppConfig}/{action=AppConfig}");
				endpoints.MapRazorPages();
			});

        }

        private class HadesCultureProvider : CustomRequestCultureProvider
        {
            public HadesCultureProvider() : base(GetCultureFunc)
			{
			}

            private static Func<HttpContext, Task<ProviderCultureResult>> GetCultureFunc => async context =>
            {
                string locale;
                Models.IUser connectedUser = Util.ConnexionUtil.CurrentUser(context.User);

                using (Data.ApplicationDbContext db = new())
				{
                    string queryStringLanguage = context.Request.Query["l"].ToString();
                    locale =
                        !string.IsNullOrWhiteSpace(queryStringLanguage) ? queryStringLanguage : null ??     // TEMP, CHECK l IN QUERY STRING. TO BE REMOVED, THIS IS AN UNFILTERED USER INPUT ENTRY POINT
                        connectedUser?.GetUserConfig().Language ??                                          // Get from connected user.
                        db.AppConfig.FirstOrDefault()?.DefaultLanguage ??                                   // Get from app config.
                        Settings.AppSettings.DefaultCulture;                                                // Get default in appsettings.json (Always defined)
                }

                // Return culture from request
                return await Task.FromResult(new ProviderCultureResult(locale));
            };
        }
    }
}