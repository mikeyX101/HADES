namespace HADES.Models
{
	public class SMTPSettings
	{
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPFromEmail { get; set; }

        public SMTPSettings(string server, int port, string username, string password, string fromEmail)
		{
            SMTPServer = server;
            SMTPPort = port;
            SMTPUsername = username;
            SMTPPassword = password;
            SMTPFromEmail = fromEmail;
        }
    }
}
