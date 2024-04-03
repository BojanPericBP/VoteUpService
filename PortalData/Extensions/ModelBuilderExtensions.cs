using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData.Extensions;

internal static class SoftDeleteModelBuilderExtensions
{
    public static ModelBuilder ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                continue;

            var param = Expression.Parameter(entityType.ClrType, "entity");
            var prop = Expression.PropertyOrField(param, nameof(ISoftDelete.Deleted));
            var entityNotDeleted = Expression.Lambda(Expression.Equal(prop, Expression.Constant(null)), param);

            entityType.SetQueryFilter(entityNotDeleted);
        }

        return modelBuilder;
    }
}