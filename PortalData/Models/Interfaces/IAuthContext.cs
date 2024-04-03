using VoteUp.PortalData.Models.Identity;

namespace VoteUp.PortalData.Models.Interfaces;

public interface IAuthContext
{
    public Guid? UserId { get; set; }
	public string? RemoteIpAddress { get; set; }
	public List<RoleName> Roles { get; set; }
	public List<string> Permissions { get; set; }
}