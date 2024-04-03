using VoteUp.PortalData;
using VoteUp.PortalData.Models.Base;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.Portal.Repositories;

public interface ICityRepository : IBaseRepository<City> { }


public class CityRepository : BaseRepository<City>, ICityRepository
{
	public CityRepository(	
		VoteUpDbContext dbContext,
		IAuthContext authContext,
		IConfiguration configuration,
		ILogger<CityRepository> logger
		) : base(dbContext, authContext, configuration, logger)
		{
			QueryFilterColumns = ["Name", "Description"];
		}
}

