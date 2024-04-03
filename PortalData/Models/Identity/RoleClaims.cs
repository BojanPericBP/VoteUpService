using Microsoft.AspNetCore.Identity;

namespace VoteUp.PortalData;

public class RoleClaims : IdentityRoleClaim<Guid>
{
    public virtual Role Role { get; set; } = default!;
}
