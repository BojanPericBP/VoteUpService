using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoteUp.PortalData.Extensions;
using VoteUp.PortalData.Models;
using VoteUp.PortalData.Models.Base;
using VoteUp.PortalData.Models.Identity;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData;

public class VoteUpDbContext
	: IdentityDbContext<User, Role, Guid, UserClaims, UserRoles, UserLogins, RoleClaims, UserTokens>
{
	public VoteUpDbContext(DbContextOptions options)
		: base(options)
	{
		Test = Guid.NewGuid();
		Console.WriteLine($"DbContext: {Test} CREATED...");
	}

	public Guid Test { get; set; } = default!;

	public IAuthContext? AuthContext { get; set; }
	public DbSet<Audit> AuditLogs { get; set; } = default!;
	public DbSet<Email> Emails { get; set; } = default!;
	public DbSet<City> Cities { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfigurationsFromAssembly(typeof(VoteUpDbContext).Assembly);
		builder.ApplySoftDeleteQueryFilter();
	}

	public override async ValueTask DisposeAsync()
	{
		await base.DisposeAsync();
		Console.WriteLine($"DbContext: {Test} DISPOSED...");
	}

	public override void Dispose()
	{
		base.Dispose();
		Console.WriteLine($"DbContext: {Test} DISPOSED...");
	}
}
