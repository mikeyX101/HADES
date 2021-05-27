using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util
{
	public static class LogManager
	{
		private enum Env
		{
			Production,
			Development
		}

		private static IServiceProvider Services { get; set; }
		private static Microsoft.Extensions.Configuration.IConfiguration Config { get; set; }
		private static Env Environment { get; set; }

		public static void Initialize(Microsoft.Extensions.Hosting.HostBuilderContext context, IServiceProvider services, LoggerConfiguration config)
		{
			Services = services;
			Config = context.Configuration;

			if (context.HostingEnvironment.EnvironmentName == "Development")
			{
				Environment = Env.Development;
			}
			else if (context.HostingEnvironment.EnvironmentName == "Production") 
			{
				Environment = Env.Production;
			}

			config
				.ReadFrom.Configuration(Config)
				.ReadFrom.Services(Services);
		}

		// Events needed to be logged will not be logged if this function is running
		public static void RefreshLogger(Models.AppConfig appConfig)
		{
			Log.CloseAndFlush();

			ILogger log;
			if (appConfig != null)
			{
				LoggerConfiguration logConfig = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
					.MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
					.MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Information);

				if (Environment == Env.Development)
				{
					logConfig = logConfig.WriteTo.Async(config =>
						config.Console(
							restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
							outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{User}] {Message:lj}{NewLine}{Exception}"
						)
					);
				}

				log = logConfig
					.WriteTo.Async(config =>
						config.File(new Serilog.Formatting.Compact.CompactJsonFormatter(), "Logs/log.json",
							restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
							rollingInterval: RollingInterval.Day,
							rollOnFileSizeLimit: false,
							fileSizeLimitBytes: appConfig.LogMaxFileSize,
							retainedFileCountLimit: appConfig.LogDeleteFrequency,
							buffered: true,
							flushToDiskInterval: TimeSpan.FromMinutes(1)))
					.Enrich.FromLogContext()
					.ReadFrom.Services(Services)
					.CreateLogger();
			}
			else
			{
				log = new LoggerConfiguration()
					.ReadFrom.Configuration(Config)
					.ReadFrom.Services(Services)
					.CreateLogger();
			}
			Log.Logger = log;
		}
	}
}
