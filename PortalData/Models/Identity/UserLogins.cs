using Microsoft.AspNetCore.Identity;

namespace VoteUp.PortalData.Models.Identity;

public class UserLogins : IdentityUserLogin<Guid>
{
	public virtual User User { get; set; } = default!;
}
