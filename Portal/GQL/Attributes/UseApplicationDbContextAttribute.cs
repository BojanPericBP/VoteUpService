// using System.Reflection;
// using HotChocolate.Types;
// using HotChocolate.Types.Descriptors;
// using VoteUp.Portal.GQL.Extensions;
// using VoteUp.PortalData;

// namespace VoteUp.Portal.GQL.Attributes;

// public class UseVoteUpDbContextAttribute : ObjectFieldDescriptorAttribute
// {
// 	protected override void OnConfigure(
// 		IDescriptorContext context,
// 		IObjectFieldDescriptor descriptor,
// 		MemberInfo member
// 	)
// 	{
// 		descriptor.UseDbContext<VoteUpDbContext>();
// 	}
// }
