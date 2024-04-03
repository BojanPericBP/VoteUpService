namespace VoteUp.PortalData.Models.Interfaces;

public interface ISoftDelete
{
	DateTime? Deleted { get; set; }
}
