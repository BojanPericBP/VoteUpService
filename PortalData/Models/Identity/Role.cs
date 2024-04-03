using Microsoft.AspNetCore.Identity;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.PortalData;

public class Role : IdentityRole<Guid>
{
    public Role() : base()
    {

    }
    public Role(string roleName) : this()
    {
        Name = roleName;
    }
    public virtual ICollection<UserRoles> Users { get; set; } = default!;
    public virtual ICollection<RoleClaims> Claims { get; set; } = default!;
}
