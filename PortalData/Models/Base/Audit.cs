using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData.Models.Base;

public class Audit : IIdentity<int>
{
	public int Id { get; set; }

	public Guid? UserId { get; set; }
	public string? TableName { get; set; }
	public DateTime Timestamp { get; set; }
	public string? KeyValues { get; set; }
	public string? OldValues { get; set; }
	public string? NewValues { get; set; }
}
