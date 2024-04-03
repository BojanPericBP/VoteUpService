using Microsoft.EntityFrameworkCore;

namespace VoteUp.Portal.Util;

public record PageRequest(int Limit, int Offset);

public record Page<T>(List<T> Data, int TotalCount);

public static class PaginationExtensions
{
	public static Page<T> GetPage<T>(this IQueryable<T> data, PageRequest pageRequest)
	{
		pageRequest = ValidatePageRequest(pageRequest);

		var count = data.Count();
		data = data.Skip(pageRequest.Offset).Take(pageRequest.Limit);

		return new Page<T>(data.ToList(), count);
	}

    public static async Task<Page<T>> GetPageAsync<T>(this IQueryable<T> data, PageRequest pageRequest)
	{
		pageRequest = ValidatePageRequest(pageRequest);

		var count = await data.CountAsync();
		data = data.Skip(pageRequest.Offset).Take(pageRequest.Limit);

		return new Page<T>(await data.ToListAsync(), count);
	}

	private static PageRequest ValidatePageRequest(PageRequest pageRequest)
	{
		if(pageRequest.Limit <= 0)
			pageRequest = pageRequest with {Limit = 25};

		if(pageRequest.Offset < 0)
			pageRequest = pageRequest with {Offset = 0};

		return pageRequest;
	}
}
