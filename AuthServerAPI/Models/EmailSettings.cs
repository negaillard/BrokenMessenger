namespace AuthServerAPI.Models
{
	public class EmailSettings
	{
		public string SmtpClientHost { get; set; } = "smtp.gmail.com";
		public int SmtpClientPort { get; set; } = 587;
		public string MailLogin { get; set; }
		public string MailPassword { get; set; }
		public string SenderName { get; set; } = "Мессенджер Олег";
		public bool EnableSsl { get; set; } = true;
	}
}
