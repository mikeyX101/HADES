using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Threading.Tasks;

namespace HADES.Middlewares
{
	public static partial class HadesMiddlewareExtensions
	{
		public static IApplicationBuilder UseUserLog(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<UserLogMiddleware>();
		}

		private sealed class UserLogMiddleware
		{
			private readonly RequestDelegate next;

			public UserLogMiddleware(RequestDelegate next)
			{
				this.next = next;
			}

			public async Task Invoke(HttpContext context)
			{
				string userName = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "unknown";
				LogContext.PushProperty("User", !string.IsNullOrWhiteSpace(userName) ? userName : "unknown");

				await next(context);
			}
		}
	}
}
