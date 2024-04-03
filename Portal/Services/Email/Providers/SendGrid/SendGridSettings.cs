namespace VoteUp.Portal.Services.Email.Providers.Sendgrid;

public class SendGridSettings : IProviderSettings
{
	public bool Paused { get; set; } = false;
	public string? ApiKey { get; set; }
	public string? DefaultFrom { get; set; }
	public string? DefaultTo { get; set; }
	public string? DefaultFromName { get; set; }
}
