using HADES.Models.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class CSPReportController : ControllerBase
	{
		[HttpPost]
		[AllowAnonymous]
		[Consumes("application/csp-report")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult Report([FromBody] CSPReportRequest report)
		{
			if (report == null || report.CSPReport == null)
			{
				return new BadRequestResult();
			}

			// Log user that made the report?
			Console.WriteLine(report.ToString()); //TODO Log

			return new OkObjectResult("OK");
		}
	}
}
