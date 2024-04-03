using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData.Models.Identity;

public class User : IdentityUser<Guid>, IIdentity<Guid>, ICMTimestamps, ISoftDelete
{
	[MaxLength(100), ProtectedPersonalData]
	public string FirstName { get; set; } = default!;

	[MaxLength(100), ProtectedPersonalData]
	public string LastName { get; set; } = default!;

	public DateTime? LastLogin { get; set; }

	[NotMapped]
	public List<string>? Permissions { get; set; }

	[NotMapped]
	public string? AuthToken { get; set; }

	public virtual ICollection<UserTokens> UserTokens { get; set; } = [];
	public virtual ICollection<UserRoles> Roles { get; set; } = [];
	public virtual ICollection<UserLogins> Logins { get; set; } = [];
	public virtual ICollection<UserClaims> Claims { get; set; } = [];

	public DateTime Created { get; set; }
	public DateTime Modified { get; set; }
	public DateTime? Deleted { get; set; }
}
