using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using HADES.Models;

namespace HADES.Util
{
	public static class FileManager
	{
		private static Dictionary<Guid, AvailableFile> Files { get; set; } = new();

		public static Guid Add(AvailableFile file)
		{
			Guid id = Guid.NewGuid();
			Files.Add(id, file);
			return id;
		}

		public static AvailableFile GetFile(Guid id)
		{
			if (Files.TryGetValue(id, out AvailableFile file))
			{
				if (file.ExpirationDate < DateTime.UtcNow)
				{
					Serilog.Log.Warning("A file has been accessed in the FileManager when it's already expired. File at {Path} has been deleted and is no longer in the FileManager", file.FilePath);
					Remove(id);
				}
				if (File.Exists(file.FilePath))
				{
					return file;
				}
				else
				{
					Serilog.Log.Warning("A file with a valid Guid stored in the FileManager is still in the FileManager, but it's physical file at {Path} doesn't exist. It has been deleted from the FileManager", file.FilePath);
					Remove(id);
				}
			}
			return null;
		}

		public static void Remove(Guid id)
		{
			if (Files.TryGetValue(id, out AvailableFile file))
			{
				if (File.Exists(file.FilePath))
				{
					File.Delete(file.FilePath);
				}
				Files.Remove(id);
			}
		}

		public static void CleanUpExpired()
		{
			Serilog.Log.Information("Running temporary file cleanup");
			foreach (Guid fileId in Files.Keys)
			{
				AvailableFile file = Files[fileId];
				if (file.ExpirationDate < DateTime.UtcNow)
				{
					Serilog.Log.Information("Removed expired temporary file ({Path}) on cleanup publicly known as {PublicName}", file.FilePath, file.PublicFileName);
					Remove(fileId);
				}
			}
		}
	}
}
