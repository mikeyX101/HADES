using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace HADES.Services
{
    public class TempFileCleanupService : IHostedService
    {
        private static Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!Directory.Exists("Temp"))
            {
                Directory.CreateDirectory("Temp");
            }

            // Clean Temp folder on start up
            string[] filePaths = Directory.GetFiles("Temp/", "*", SearchOption.AllDirectories);
            foreach(string path in filePaths)
			{
                try
				{
                    File.Delete(path);
                    Serilog.Log.Information("Deleted old temporary {EntryType} {Path} from temporary folder while starting TempCleanupService.", "File", path);
                }
                catch (Exception e)
				{
                    Serilog.Log.Warning(e, "Couldn't delete {EntryType} {Path} from temporary folder while starting TempCleanupService.", "File", path);
				}
			}

            string[] directoryPaths = Directory.GetDirectories("Temp/", "*", SearchOption.AllDirectories);
            foreach (string path in directoryPaths)
            {
                try
                {
                    Directory.Delete(path);
                    Serilog.Log.Information("Deleted old temporary {EntryType} {Path} from temporary folder while starting TempCleanupService.", "Directory", path);
                }
                catch (Exception e)
                {
                    Serilog.Log.Warning(e, "Couldn't delete {EntryType} {Path} from temporary folder while starting TempCleanupService.", "Directory", path);
                }
                
            }

            _timer = new Timer(
                _ => Util.FileManager.CleanUpExpired(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(Settings.AppSettings.TempCleanupIntervalMins)
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
