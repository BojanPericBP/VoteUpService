using HotChocolate.Authorization;

namespace VoteUp.Portal.GQL.Controllers;

[Authorize]
public class Query
{
	public Test TestQuery(string arg1, string? arg2)
	{
		return arg2 is not null
			? new Test(Guid.NewGuid(), arg2)
			: new Test(Guid.NewGuid(), arg1);
	}
}

public record Test(Guid Id, string Name);
