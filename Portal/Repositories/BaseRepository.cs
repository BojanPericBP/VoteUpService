using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VoteUp.Portal.Exceptions;
using VoteUp.Portal.Util;
using VoteUp.PortalData;
using VoteUp.PortalData.Extensions;
using VoteUp.PortalData.Models.Identity.Constants;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.Portal.Repositories;

public interface IBaseRepository<T>
	where T : class
{
	Task<T?> CreateAsync(T data);
	Task<T?> UpdateAsync(dynamic data);
	Task<T?> UpdateAsync(T data);
	Task<T?> DeleteAsync(object id);
	Task<T?> RestoreAsync(object id);
	Task<T?> GetByIdAsync(object id, bool ignoreQueryFilters = false);
	T? GetById(object id, bool ignoreQueryFilters = false);
	Task<Page<T>?> GetAllAsync(PageRequest pageData, FilterRequest? filterRequest);
}

public class BaseRepository<T>(
	VoteUpDbContext dbContext,
	IAuthContext? authContext,
	IConfiguration? configuration,
	ILogger<BaseRepository<T>>? logger
) : IBaseRepository<T>
	where T : class
{
	protected VoteUpDbContext _db = dbContext;
	protected List<string>? QueryFilterColumns = [];
	protected IConfiguration? _config = configuration;
	protected readonly ILogger? _logger = logger;
	protected readonly IAuthContext? _authContext = authContext;

	// private bool _disposed = false;

	public BaseRepository(VoteUpDbContext dbContext, ILogger<BaseRepository<T>>? logger = null)
		: this(dbContext: dbContext, authContext: null, configuration: null, logger: logger) { }

	public BaseRepository(
		VoteUpDbContext dbContext,
		IAuthContext? authContext,
		ILogger<BaseRepository<T>>? logger = null
	)
		: this(dbContext: dbContext, authContext: authContext, configuration: null, logger: logger)
	{ }

	public virtual async Task<T?> CreateAsync(T data)
	{
		if (typeof(IOwnedByUser).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
		{
			if (_authContext is null || !_authContext.UserId.HasValue)
				throw new ApiException("Missing User.");

			((IOwnedByUser)data).OwnedByUserId = _authContext.UserId.Value;
		}

		_db.Add(data);
		await _db.SaveChangesAsync();

		return await RefetchData(data);
	}

	public virtual async Task<T?> UpdateAsync(dynamic data)
	{
		// data = RemoveTimestamps(data);

		var json = JsonSerializer.Serialize(data);
		object id = GetPrimaryKey(json);

		var dbData = await _db.FindAsync<T>(id) ?? throw new ApiException("Can't find data", ErrorCode.NotFound.ToString());
		Newtonsoft.Json.JsonConvert.PopulateObject(json, dbData);

		if (typeof(IOwnedByUser).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
		{
			if (_authContext is null || !_authContext.UserId.HasValue)
				throw new ApiException("Missing User.");

			if (((IOwnedByUser)dbData).OwnedByUserId != _authContext.UserId.Value)
				throw new ApiException("You do not have permission to update this item.");
		}

		_db.Update(dbData);
		await _db.SaveChangesAsync();

		_db.Entry(dbData).State = EntityState.Detached;
		return await _db.FindAsync<T>(id);
	}

	public virtual async Task<T?> UpdateAsync(T data)
	{
		//this updates all columns, if not set default value will be set
		// _db.Update(data);
		object id = GetPrimaryKey(data);
		T dbData = await GetByIdAsync(id) ?? throw new ApiException("Can't find data", ErrorCode.NotFound.ToString());

		if (typeof(IOwnedByUser).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
		{
			if (_authContext is null || !_authContext.UserId.HasValue)
				throw new ApiException("Missing User.");

			if (((IOwnedByUser)dbData).OwnedByUserId != _authContext.UserId.Value)
				throw new ApiException("You do not have permission to update this item.");

			((IOwnedByUser)data).OwnedByUserId = _authContext.UserId.Value;
		}

		_db.Entry(dbData).CurrentValues.SetValues(data);

		await _db.SaveChangesAsync();
		return await RefetchData(data);
	}

	public virtual async Task<T?> DeleteAsync(object id)
	{
		T data = await GetByIdAsync(id) ?? throw new ApiException("Record doesn't exist.");

		_db.Remove<T>(data);
		await _db.SaveChangesAsync();
		return data;
	}

	public virtual async Task<T?> RestoreAsync(object id)
	{
		T data = await GetByIdAsync(id, true) ?? throw new ApiException("Record doesn't exist.", ErrorCode.NotFound.ToString());

		((ISoftDelete)data).Deleted = null;
		_db.Update(data);
		await _db.SaveChangesAsync();
		return data;
	}

	private IQueryable<T> GetByIdQueryable(object id, bool ignoreQueryFilters = false)
	{
		var data = _db.Set<T>().AsQueryable();

		if (ignoreQueryFilters)
			data = data.IgnoreQueryFilters();

		if (typeof(IOwnedByUser).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
		{
			if (_authContext is null || !_authContext.UserId.HasValue)
				throw new ApiException("Missing User.");

			if (!_authContext.Permissions.Contains(Permission.AllUsersManage))
				data = data.Where(x =>
					EF.Property<Guid>(x, nameof(IOwnedByUser.OwnedByUserId))
					== _authContext.UserId.Value
				);
		}

		return data;
	}

	public virtual async Task<T?> GetByIdAsync(object id, bool ignoreQueryFilters = false)
	{
		if (id is null)
			return null;

		var data = GetByIdQueryable(id, ignoreQueryFilters);

		if (typeof(IIdentity<int>).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			return await data.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == (int)id);

		return await data.FirstOrDefaultAsync(x =>
			EF.Property<Guid>(x, "Id") == Guid.Parse(id.ToString()!)
		);
	}

	public virtual T? GetById(object id, bool ignoreQueryFilters = false)
	{
		if (id == null)
			return null;

		var data = GetByIdQueryable(id, ignoreQueryFilters);

		if (typeof(IIdentity<int>).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			return data.FirstOrDefault(x => EF.Property<int>(x, "Id") == (int)id);

		return data.FirstOrDefault(x => EF.Property<Guid>(x, "Id") == Guid.Parse(id.ToString()!));
	}

	public virtual async Task<Page<T>?> GetAllAsync(
		PageRequest pageRequest,
		FilterRequest? filterRequest
	)
	{
		return await DoBasePagination(DoBaseFiltering(_db.Set<T>(), filterRequest), pageRequest);
	}

	public virtual async Task<Page<T>> DoBasePagination(
		IQueryable<T> data,
		PageRequest pageRequest
	) => await data.GetPageAsync(pageRequest);

	// TODO maybe refactor method to prefiltering extensions for sorting and column filtering

	protected virtual IQueryable<T> DoBaseFiltering(
		IQueryable<T> data,
		FilterRequest? filterRequest
	)
	{
		if (
			!string.IsNullOrEmpty(filterRequest?.SortBy)
			&& !string.IsNullOrEmpty(filterRequest?.SortOrder)
		)
			data = data.SortByColumnName(filterRequest.SortBy, filterRequest.SortOrder);
		else if (typeof(ICMTimestamps).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			data = data.OrderByDescending(x => ((ICMTimestamps)x).Created);

		if (!string.IsNullOrEmpty(filterRequest?.Query) && QueryFilterColumns?.Any() == true)
		{
			var firstData = data;
			var query = filterRequest.Query.ToLower();

			// foreach (var queryFilterColumn in QueryFilterColumns)
			for (int i = 0; i < QueryFilterColumns.Count; i++)
			{
				string columnName = QueryFilterColumns[i];

				if (i == 0)
					data = data.Where(x =>
						EF.Property<string>(x, columnName).ToLower().Contains(query)
					);
				else
					data = data.Union(
						firstData.Where(x =>
							EF.Property<string>(x, columnName).ToLower().Contains(query)
						)
					);
			}
		}

		var filterColumn = nameof(ICMTimestamps.Created);
		if (
			filterRequest is not null
			&& filterRequest.ShowDeleted.HasValue
			&& filterRequest.ShowDeleted.Value
		)
		{
			data = data.IgnoreQueryFilters();
			filterColumn = nameof(ISoftDelete.Deleted);
		}

		if (filterRequest is not null && filterRequest.StartDate.HasValue)
			data = data.Where(x =>
				EF.Property<DateTime>(x, filterColumn) >= filterRequest.StartDate
			);

		if (filterRequest is not null && filterRequest.EndDate.HasValue)
			data = data.Where(x => EF.Property<DateTime>(x, filterColumn) <= filterRequest.EndDate);

		if (typeof(IOwnedByUser).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
		{
			if (_authContext is null || !_authContext.UserId.HasValue)
				throw new ApiException("Missing User.");

			if (!_authContext.Permissions.Contains(Permission.AllUsersManage))
				data = data.Where(x =>
					EF.Property<Guid>(x, nameof(IOwnedByUser.OwnedByUserId))
					== _authContext.UserId.Value
				);
		}

		return data;
	}

	protected async Task<T?> RefetchData(T data)
	{
		object id = GetPrimaryKey(data);
		_db.Entry(data).State = EntityState.Detached;
		return await _db.FindAsync<T>(id);
	}

	protected object GetPrimaryKey(T data)
	{
		Type t = data.GetType();
		PropertyInfo prop = t.GetProperty("Id")!;
		object id = prop.GetValue(data)!;
		return id;
	}

	protected object GetPrimaryKey(string data)
	{
		T t = JsonSerializer.Deserialize<T>(
			data,
			new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
		)!;
		return GetPrimaryKey(t);
	}

	protected async Task<T?> RefetchData(dynamic data, object id)
	{
		if (id == null)
			id = GetPrimaryKey(data);

		_db.Entry(data).State = EntityState.Detached;
		return await _db.FindAsync<T>(id);
	}
}
