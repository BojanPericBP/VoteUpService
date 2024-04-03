using Microsoft.AspNetCore.Identity;

namespace VoteUp.PortalData.Models.Identity;

public class UserClaims : IdentityUserClaim<Guid>
{
	public virtual User User { get; set; } = default!;
}
