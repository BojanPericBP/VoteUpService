using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VoteUp.PortalData.Models.Interfaces;

namespace Controls.Data.Interceptors;

public sealed class CMTimestampInterceptor : SaveChangesInterceptor
{
	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default
	)
	{
		DbContext? dbContext = eventData.Context;

		if (dbContext is null)
		{
			return base.SavingChangesAsync(eventData, result, cancellationToken);
		}

		var entries = dbContext.ChangeTracker.Entries<ICMTimestamps>();
		var now = DateTime.UtcNow;

		SetCMFlags(entries, now);

		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	public override InterceptionResult<int> SavingChanges(
		DbContextEventData eventData,
		InterceptionResult<int> result
	)
	{
		DbContext? dbContext = eventData.Context;

		if (dbContext is null)
		{
			return base.SavingChanges(eventData, result);
		}

		var entries = dbContext.ChangeTracker.Entries<ICMTimestamps>();
		var now = DateTime.UtcNow;

		SetCMFlags(entries, now);

		return base.SavingChanges(eventData, result);
	}

	private static void SetCMFlags(IEnumerable<EntityEntry<ICMTimestamps>> entries, DateTime now)
	{
		foreach (var entityEntry in entries)
		{
			if (entityEntry.State == EntityState.Added)
			{
				entityEntry.Property(a => a.Created).CurrentValue = now;
				entityEntry.Property(a => a.Modified).CurrentValue = now;
			}
			else if (entityEntry.State == EntityState.Modified)
			{
				entityEntry.Property(a => a.Modified).CurrentValue = now;
			}
		}
	}
}
