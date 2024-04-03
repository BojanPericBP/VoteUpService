using VoteUp.Portal.Services.Email.Util;

namespace VoteUp.Portal.Services.Email.Providers;

public interface IEmailProvider
{
    Task<EmailSendingResult> SendAsync(PortalData.Models.Email message);
}