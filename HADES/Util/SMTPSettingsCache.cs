namespace HADES.Util
{
    public static class SMTPSettingsCache
    {
        public static Models.SMTPSettings SMTP { get; set; }

        public async static void Refresh()
        {
			Services.AppConfigService service = new();
            SMTP = await service.getSMTPInfo();
        }
    }
}
