using HotChocolate.Authorization;
using VoteUp.Portal.DTO;
using VoteUp.Portal.Repositories;
using VoteUp.Portal.Util;
using VoteUp.PortalData.Models.Base;
using VoteUp.PortalData.Models.Identity.Constants;

namespace VoteUp.Portal.GQL.Controllers;

[ExtendObjectType(typeof(Query))]
public class QueryCityResolvers
{
	[Authorize(Policy = Permission.ReadCity)]
	public async Task<Page<City>?> GetCitiesAsync(
		[Service(ServiceKind.Synchronized)] ICityRepository cityRepository,
		PageRequest pageRequest,
		FilterRequest? filterRequest
	)
	{
		return await cityRepository.GetAllAsync(pageRequest, filterRequest);
	}

	[Authorize(Policy = Permission.ReadCity)]
	public async Task<City?> GetCityAsync(
		[Service(ServiceKind.Synchronized)] ICityRepository cityRepository,
		[ID] Guid id
	)
	{
		return await cityRepository.GetByIdAsync(id);
	}
}

[ExtendObjectType(typeof(Mutation))]
public class MutationCityResolvers
{
	[Authorize(Policy = Permission.CreateCity)]
	public async Task<City?> CreateCityAsync(
		[Service(ServiceKind.Synchronized)] ICityRepository cityRepository,
		CityInput item
	)
	{
		return await cityRepository.CreateAsync(item.MapToCreate());
	}

	[Authorize(Policy = Permission.UpdateCity)]
	public async Task<City?> UpdateCityAsync(
		[Service(ServiceKind.Synchronized)] ICityRepository cityRepository,
		CityInput item
	)
	{
		return await cityRepository.UpdateAsync(item.MapToUpdate());
	}

	[Authorize(Policy = Permission.DeleteCity)]
	public async Task<City?> DeleteCityAsync(
		[Service(ServiceKind.Synchronized)] ICityRepository cityRepository,
		[ID] Guid id
	)
	{
		return await cityRepository.DeleteAsync(id);
	}

	[Authorize(Policy = Permission.RestoreCity)]
	public async Task<City?> RestoreCityAsync(
		[Service(ServiceKind.Synchronized)] ICityRepository cityRepository,
		[ID] Guid id
	)
	{
		return await cityRepository.RestoreAsync(id);
	}
}

[ExtendObjectType(typeof(Subscription))]
public class SubscriptionCityResolvers
{
	public string CreatedCityEvent()
	{
		return "Created";
	}
}
