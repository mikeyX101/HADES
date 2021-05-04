using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Models.ViewModels;

namespace HADES.Controllers
{
	public class ErrorsController : Controller
	{
		[Route("/Errors/{code}")]
		public IActionResult Error(string code = null)
		{
			if (code == null)
			{
				code = HttpContext.Response.StatusCode.ToString();
			}

			HttpStatusErrorViewModel model = new();

			switch (code)
			{
				case "404":
					model.PageTitle = "404";
					model.StatusNumber = "404";
					model.StatusName = "test 404";
					model.StatusDescription = "test 404 description";
					break;
				default:
					model.PageTitle = "unknown";
					model.StatusName = "test unknown";
					model.StatusDescription = "test unknown description";
					break;
			}

			return View(model);
		}
	}
}
