using Microsoft.AspNetCore.Identity;

namespace VoteUp.PortalData.Models.Identity;

public class UserTokens : IdentityUserToken<Guid>
{
	public virtual User User { get; set; } = default!;
}
