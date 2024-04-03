using Microsoft.EntityFrameworkCore;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.PortalData;

public class VoteUpDbContextFactory(IDbContextFactory<VoteUpDbContext> pooledFactory, IAuthContext authContext)
	: IDbContextFactory<VoteUpDbContext>
{
	private readonly IDbContextFactory<VoteUpDbContext> _pooledFactory = pooledFactory;

	public VoteUpDbContext CreateDbContext()
	{
		var context = _pooledFactory.CreateDbContext();
		context.AuthContext = authContext;
		
		return context;
	}
}
