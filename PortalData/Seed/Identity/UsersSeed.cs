using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VoteUp.PortalData.Helpers;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.PortalData.Seed.Identity;

public static class UsersSeed
{
	public static VoteUpDbContext SeedUsers(
		this VoteUpDbContext db,
		UserManager<User> userManager,
		IOptions<List<InitialUserConfiguration>> initUsersConfOpt
	)
	{
		var initialUsers = initUsersConfOpt.Value;

		if (initialUsers is null)
			return db;

		var newUsers = initialUsers
			.Where(x =>
				x.Username != null
				&& userManager.FindByNameAsync(x.Username).GetAwaiter().GetResult() == null
			)
			.ToList();

		newUsers.ForEach(user =>
		{
			User newUser =
				new()
				{
					UserName = user.Username,
					Email = user.Email,
					FirstName = user.FirstName ?? "",
					LastName = user.LastName ?? "",
				};
			IdentityResult result = userManager
				.CreateAsync(newUser, user.Password!)
				.GetAwaiter()
				.GetResult();

			if (result.Succeeded && user.Roles != null)
			{
				foreach (var role in user.Roles)
					userManager.AddToRoleAsync(newUser, role).Wait();
			}
		});

		return db;
	}
}
