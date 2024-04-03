using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData.Models;

public class Email : IIdentity<Guid>, ICMTimestamps
{
	public Guid Id { get; set; }
	public string Subject { get; set; } = string.Empty;
	public string HtmlContent { get; set; } = string.Empty;

	public string Sender { get; set; } = string.Empty;
	public string Receiver { get; set; } = string.Empty;
	public DateTime? Sent { get; set; }
	public Guid? SenderUserId { get; set; }

	public DateTime Created { get; set; }
	public DateTime Modified { get; set; }
}
