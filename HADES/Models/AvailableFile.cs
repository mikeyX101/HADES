using System;

namespace HADES.Models
{
	public class AvailableFile
    {
        public AvailableFile(string path, string publicName, string mimeType, int ownerID)
		{
            ExpirationDate = DateTime.UtcNow.AddMinutes(Settings.AppSettings.TempFileExpiresMins);
            FilePath = path;
            PublicFileName = publicName;
            MimeType = mimeType;
            OwnerID = ownerID;
        }

        public DateTime ExpirationDate { get; private set; }
        public string FilePath { get; private set; }
        public string PublicFileName { get; private set; }
        public string MimeType { get; private set; }
        public int OwnerID { get; private set; }
    }
}
