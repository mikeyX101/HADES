using HADES.Models;
using HADES.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Controllers
{
	public class HomeController : Controller
	{

		public HomeController()
		{
			ADManager ad = new ADManager();
			Console.WriteLine(ad.Validate());
		}

		public IActionResult Login()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
