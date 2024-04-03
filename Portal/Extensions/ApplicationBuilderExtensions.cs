using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VoteUp.PortalData;
using VoteUp.PortalData.Helpers;
using VoteUp.PortalData.Models.Identity;
using VoteUp.PortalData.Seed.Identity;

namespace VoteUp.Portal.Exceptions;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder MigrateDb(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();

		var context = scope.ServiceProvider.GetRequiredService<VoteUpDbContext>();

		context.Database.Migrate();

		var initUserConfOpt = scope
			.ServiceProvider
			.GetRequiredService<IOptions<List<InitialUserConfiguration>>>();
		var userManager = scope
			.ServiceProvider
			.GetRequiredService<UserManager<User>>();
		var roleManager = scope
			.ServiceProvider
			.GetRequiredService<RoleManager<Role>>();

		context.SeedRoles(roleManager).SeedUsers(userManager, initUserConfOpt);

		return app;
	}
}
