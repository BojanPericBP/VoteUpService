using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VoteUp.Portal.Exceptions;
using VoteUp.PortalData;
using VoteUp.PortalData.Models.Identity;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.Portal.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
	Task<User> CreateWithPasswordAsync(User item, string password);
	Task<User?> GetByUsernameAsync(string username);
	Task SaveLastLoginAsync(User user, DateTime timestamp);
}

public class UserRepository(
	VoteUpDbContext dbContext,
	IAuthContext? authContext,
	ILogger<UserRepository> logger,
	UserManager<User> _userManager
) : BaseRepository<User>(dbContext, authContext, logger: logger), IUserRepository
{
	public async Task<User?> GetByUsernameAsync(string username)
	{
		return await _db
			.Users.Where(x => x.Email == username || x.UserName == username)
			.FirstOrDefaultAsync();
	}

	public override async Task<User?> CreateAsync(User data)
	{
		IdentityResult result = await _userManager.CreateAsync(data);
		if (!result.Succeeded)
		{
			_logger!.LogError(
				"Error during creation of user: {email}. Error: {message}",
				data.Email,
				string.Join(',', result.Errors)
			);
			throw new ApiException("User creation failed");
		}

		return await GetByUsernameAsync(data.UserName!);
	}

	public async Task<User> CreateWithPasswordAsync(User item, string password)
	{
        IdentityResult result = await _userManager.CreateAsync(item, password);
		if (!result.Succeeded)
		{
			_logger!.LogError(
				"Error during creation of user: {email}. Error: {message}",
				item.Email,
				string.Join(',', result.Errors)
			);
			throw new ApiException("User creation failed");
		}

		return (await GetByUsernameAsync(item.UserName!))!;
	}

	public async Task SaveLastLoginAsync(User user, DateTime timestamp)
	{
		user.LastLogin = timestamp;
		await _db.SaveChangesAsync();
	}
}
