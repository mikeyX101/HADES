using HADES.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HADES.Controllers
{
	[AllowAnonymous]
	public class ErrorsController : LocalizedController<ErrorsController>
	{
		public ErrorsController(IStringLocalizer<ErrorsController> localizer) : base(localizer)
		{ }

		[AllowAnonymous]
		[Route("/Errors/{code}")]
		public IActionResult Error(string code = null)
		{
			if (code == null)
			{
				code = HttpContext.Response.StatusCode.ToString();
			}

			HttpStatusErrorViewModel model = new()
			{
				PageTitle = $"{code} - {Localizer[code + "Name"]}",
				StatusNumber = code,
				StatusName = Localizer[code + "Name"],
				StatusDescription = Localizer[code + "Description"]
			};

			if (model.StatusName == code + "Name" || model.StatusDescription == code + "Description")
			{
				model.PageTitle = Localizer["UnknownName"];
				model.StatusNumber = "-1";
				model.StatusName = Localizer["UnknownName"];
				model.StatusDescription = Localizer["UnknownDescription"];
			}

			return View(model);
		}
	}
}
