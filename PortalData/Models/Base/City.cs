using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData.Models.Base;

public class City : IIdentity<Guid>, ICMTimestamps, ISoftDelete, IOwnedByUser
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public int Order { get; set; } = 100;
	public string? Description { get; set; }
    public Guid OwnedByUserId { get; set ; }

	public DateTime Created { get; set; }
	public DateTime Modified { get; set; }
	public DateTime? Deleted { get; set; }
}
