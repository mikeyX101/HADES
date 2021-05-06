using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HADES.Middlewares
{
	public static partial class HadesMiddlewareExtensions
	{
		public static IApplicationBuilder UseHadesErrorHandling(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<HttpErrorHandlingMiddleware>();
		}

		private sealed class HttpErrorHandlingMiddleware
		{
			private readonly RequestDelegate _next;

			public HttpErrorHandlingMiddleware(RequestDelegate next)
			{
				_next = next;
			}

			public async Task Invoke(HttpContext context)
			{
				await _next(context);
				// TODO Only use for pages, not for general error codes
				if (context.Request.Method == "GET" && context.Response.StatusCode >= 400 && context.Response.StatusCode <= 599)
				{
					context.Request.Path = $"/Errors/{context.Response.StatusCode}";
					await _next(context);
				}
			}
		}
	}
}
