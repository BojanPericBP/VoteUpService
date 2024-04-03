// using HotChocolate.Types;
// using Microsoft.EntityFrameworkCore;

// namespace VoteUp.Portal.GQL.Extensions;

// public static class ObjectFieldDescriptorExtensions
// {
// 	public static IObjectFieldDescriptor UseDbContext<TDbContext>(
// 		this IObjectFieldDescriptor descriptor
// 	)
// 		where TDbContext : DbContext
// 	{
// 		return descriptor.UseScopedService(
// 			create: s => s.GetRequiredService<IDbContextFactory<TDbContext>>().CreateDbContext(),
// 			disposeAsync: (s, c) => c.DisposeAsync()
// 		);
// 	}
// }
