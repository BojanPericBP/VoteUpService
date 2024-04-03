using Microsoft.AspNetCore.Identity;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.PortalData.Seed.Identity;

public static class RolesSeed
{
	public static VoteUpDbContext SeedRoles(this VoteUpDbContext db, RoleManager<Role> roleManager)
	{
		foreach (RoleName roleName in Enum.GetValues(typeof(RoleName)))
		{
			if (!roleManager.RoleExistsAsync(roleName.ToString()).GetAwaiter().GetResult())
				roleManager.CreateAsync(new Role { Name = roleName.ToString() }).Wait();
		}

		return db;
	}
}
