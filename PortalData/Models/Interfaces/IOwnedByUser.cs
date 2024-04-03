namespace VoteUp.PortalData.Models.Interfaces;

public interface IOwnedByUser
{
	Guid OwnedByUserId { get; set; }
}
