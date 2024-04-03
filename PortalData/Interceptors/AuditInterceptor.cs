using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VoteUp.PortalData.Helpers.Audits;
using VoteUp.PortalData.Models.Base;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData.Interceptors;

public sealed class AuditInterceptor() : SaveChangesInterceptor
{
	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default
	)
	{
		DbContext? dbContext = eventData.Context;

		var authContext = dbContext is VoteUpDbContext voteUpDbContextAuth ? voteUpDbContextAuth.AuthContext : null;

		if (dbContext is null)
		{
			return base.SavingChangesAsync(eventData, result, cancellationToken);
		}

		DoAuditChangesOnEntities(dbContext, authContext);

		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	public override InterceptionResult<int> SavingChanges(
		DbContextEventData eventData,
		InterceptionResult<int> result
	)
    {
        DbContext? dbContext = eventData.Context;

        var authContext = dbContext is VoteUpDbContext voteUpDbContextAuth ? voteUpDbContextAuth.AuthContext : null;

        if (dbContext is null)
        {
            return base.SavingChanges(eventData, result);
        }

        DoAuditChangesOnEntities(dbContext, authContext);

        return base.SavingChanges(eventData, result);
    }

    private static void DoAuditChangesOnEntities(DbContext dbContext, IAuthContext? authContext)
    {
        dbContext.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (
                entry.Entity is Audit
                || entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged
            )
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Metadata.GetTableName() ?? "TABLE NAME MISSING",
                UserId = authContext?.UserId
            };
            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        break;

                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            if (
                                property.OriginalValue != null
                                    && property.CurrentValue != null
                                    && !property.OriginalValue.Equals(property.CurrentValue)
                                || (property.OriginalValue == null ^ property.CurrentValue == null)
                            )
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue!;
                                auditEntry.NewValues[propertyName] = property.CurrentValue!;
                            }
                        }
                        break;
                }
            }
        }

        foreach (var auditEntry in auditEntries.Where(x => !x.HasTemporaryProperties))
        {
            if (dbContext is VoteUpDbContext voteUpDbContext)
                voteUpDbContext.AuditLogs.Add(auditEntry.ToAudit());
        }
    }
}
