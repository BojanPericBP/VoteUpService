using VoteUp.Portal.Services.Email.Providers;
using VoteUp.Portal.Services.Email.Util;

namespace VoteUp.Portal.Services.Email;

public interface IEmailService
{
	EmailSendingResult Send(PortalData.Models.Email message);
	Task<EmailSendingResult> SendAsync(PortalData.Models.Email message);
}

public class EmailService(IEmailProvider emailProvider) : IEmailService
{
	private readonly IEmailProvider _emailProvider = emailProvider;

    public EmailSendingResult Send(PortalData.Models.Email message)
	{
		return _emailProvider.SendAsync(message).GetAwaiter().GetResult();
	}

	public async Task<EmailSendingResult> SendAsync(PortalData.Models.Email message)
	{
		return await _emailProvider.SendAsync(message);
	}
}
