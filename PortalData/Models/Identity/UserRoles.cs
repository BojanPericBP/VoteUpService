using Microsoft.AspNetCore.Identity;

namespace VoteUp.PortalData.Models.Identity;

public class UserRoles : IdentityUserRole<Guid>
{
	public virtual User User { get; set; } = default!;
	public virtual Role Role { get; set; } = default!;
}
