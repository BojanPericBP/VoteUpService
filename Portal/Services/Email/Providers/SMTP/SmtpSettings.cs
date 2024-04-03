namespace VoteUp.Portal.Services.Email.Providers.SMTP;

public class SmtpSettings : IProviderSettings
{
	public bool Paused { get; set; } = false;
	public string? Username { get; set; }
	public string? Password { get; set; }
	public string Host { get; set; } = string.Empty;
	public int Port { get; set; } = 25;
	public bool EnableSsl { get; set; } = false;
	public string? DefaultFrom { get; set; }
	public string? DefaultTo { get; set; }
}
