using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData.Interceptors;

public sealed class SoftDeleteEntitiesInterceptor : SaveChangesInterceptor
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

		SetDeletedFlag(dbContext.ChangeTracker.Entries<ISoftDelete>());

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

		SetDeletedFlag(dbContext.ChangeTracker.Entries<ISoftDelete>());

		return base.SavingChanges(eventData, result);
	}

	private static void SetDeletedFlag(IEnumerable<EntityEntry<ISoftDelete>> entries)
	{
		DateTime now = DateTime.UtcNow;
		
		foreach (var entityEntry in entries)
		{
			if (entityEntry.State == EntityState.Deleted)
			{
				entityEntry.Property(a => a.Deleted).CurrentValue = now;
				entityEntry.State = EntityState.Modified;
			}
		}
	}
}
