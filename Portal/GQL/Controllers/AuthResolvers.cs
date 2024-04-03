using HotChocolate.Authorization;
using VoteUp.Portal.DTO;
using VoteUp.Portal.Repositories;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.Portal.GQL.Controllers;

[ExtendObjectType(typeof(Mutation))]
public class MutationAuthResolvers
{
	[AllowAnonymous] // Opt-out GraphQL operation from authentication middleware
	public async Task<User> LoginUserAsync(
		[Service(ServiceKind.Synchronized)] IAuthRepository authRepository,
		LoginRequest request
	)
	{
		return await authRepository.LoginAsync(request.Username, request.Password);
	}
}
