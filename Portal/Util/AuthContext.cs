using VoteUp.PortalData.Models.Identity;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.Portal.Util;

public class AuthContext : IAuthContext
{
	public Guid? UserId { get; set; }
	public string? RemoteIpAddress { get; set; }
	public List<RoleName> Roles { get; set; } = [];
	public List<string> Permissions { get; set; } = [];
};
