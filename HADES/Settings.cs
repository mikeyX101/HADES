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
				throw new InvalidOperationException("Setting must be initiated before using it. Use Settings.Initiate() on app startup.");
			}
		}

		public readonly string SqlLiteConnectionString;

		private Settings(IConfiguration config)
		{
			SqlLiteConnectionString = config.GetConnectionString("DefaultConnection");
		}

		public static void Initiate(IConfiguration config)
		{
			appSettings = new Settings(config);
		}
	}
}
