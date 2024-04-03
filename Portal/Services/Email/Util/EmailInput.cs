namespace VoteUp.Portal.Services.Email.Util;

public record EmailInput(string Subject, string HtmlContent, string Receiver)
{
	public PortalData.Models.Email MapToEmail(Guid? userId = null, DateTime? sent = null) =>
		new()
		{
			Subject = Subject,
			HtmlContent = HtmlContent,
			Receiver = Receiver,
			Sent = sent,
			SenderUserId = userId
		};
}
