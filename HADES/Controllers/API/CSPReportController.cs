using HADES.Models.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HADES.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class CSPReportController : ControllerBase
	{
		// report is user input, be cautious with the data
		[HttpPost]
		[AllowAnonymous]
		[Consumes("application/csp-report")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult Report([FromBody] CSPReportRequest reportRequest)
		{
			if (reportRequest == null || reportRequest.CSPReport == null)
			{
				return new BadRequestResult();
			}

			CSPReport report = reportRequest.CSPReport;
			Serilog.Log.Information("Content Security Policy Violation: Blocked Uri {BlockedUri} in page {DocumentUri} because of {ViolatedDirective}",
				report.BlockedUri,
				report.DocumentUri,
				report.ViolatedDirective);

			return new OkObjectResult("OK");
		}
	}
}
