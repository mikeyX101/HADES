using Microsoft.Extensions.Configuration;
using System;

namespace HADES
{
	public class Settings
	{
		private static Settings appSettings;
		public static Settings AppSettings {
			get {
				if (appSettings != null)
				{
					return appSettings;
				}
				throw new InvalidOperationException("Settings must be initiated before using it. Use Settings.Initiate() on app startup.");
			}
		}

		public readonly string SqlLiteConnectionString;
		public readonly string LocalResourcesPath;
		public readonly string DefaultCulture;
		public readonly uint TempFileExpiresMins; 
		public readonly uint TempCleanupIntervalMins;

		private Settings(IConfiguration config)
		{
			SqlLiteConnectionString = config.GetConnectionString("DefaultConnection");
			LocalResourcesPath = config.GetValue<string>("LocalResourcesPath");
			DefaultCulture = config.GetValue<string>("DefaultCulture");
			TempFileExpiresMins = config.GetValue<uint>("TempFileExpiresMins");
			TempCleanupIntervalMins = config.GetValue<uint>("TempCleanupIntervalMins");
		}

		public static void Initiate(IConfiguration config)
		{
			appSettings = new Settings(config);
		}
	}
}
