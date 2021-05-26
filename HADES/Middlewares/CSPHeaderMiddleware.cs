using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HADES.Middlewares
{
	public static partial class HadesMiddlewareExtensions
	{
		public static IApplicationBuilder UseContentSecurityPolicyHeader(this IApplicationBuilder builder, string policy, string reportUri = null)
		{
			return builder.UseMiddleware<CSPHeaderMiddleware>(policy, reportUri);
		}

		private sealed class CSPHeaderMiddleware
		{
			private readonly RequestDelegate next;
			private readonly string policy;
			private readonly string reportUri;

			public CSPHeaderMiddleware(RequestDelegate next, string policy, string reportUri)
			{
				this.next = next;
				this.policy = policy;
				this.reportUri = reportUri;

				if (this.reportUri != null)
				{
					/* report-uri is deprecated, setting endpoints and specifying them to report-to is the new way to report
					 * See: https://w3c.github.io/reporting/ and https://w3c.github.io/webappsec-csp/#directives-reporting
					 *
					 * Section 6.4.1 report-uri (Taken 07-05-2021 / DD-MM-YYYY)
					 * Note: The report-uri directive is deprecated. 
					 * Please use the report-to directive instead. 
					 * If the latter directive is present, this directive will be ignored. 
					 * To ensure backwards compatibility, we suggest specifying both, like this: 
					 */
					this.policy += $"; report-uri {reportUri}; report-to csp-endpoint";
				}
			}

			public async Task Invoke(HttpContext context)
			{
				if (reportUri != null)
				{
					context.Response.Headers.Add("Reporting-Endpoints", $"csp-endpoint=\"{reportUri}\"");
				}

				context.Response.Headers.Add("Content-Security-Policy", policy);

				await next(context);
			}
		}
	}
}
