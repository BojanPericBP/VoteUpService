using Microsoft.AspNetCore.Identity;
using VoteUp.Portal.Exceptions;
using VoteUp.Portal.Services;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.Portal.Repositories;

public interface IAuthRepository
{
	Task<User> LoginAsync(string username, string password);
}

public class AuthRepository(
    IUserRepository userRepository,
    SignInManager<User> signInManager,
    IJwtService jwtService
    ) : IAuthRepository
{

    public async Task<User> LoginAsync(string username, string password)
	{
		User? user =
			await userRepository.GetByUsernameAsync(username)
			?? throw new ApiException(
				"Korisničko ime ili lozinka su pogrešni.",
				ErrorCode.InvalidCredentials.ToString()
			);

		if (user.Roles == null || user.Roles.Count == 0)
			throw new ApiException(
				"Pristup sistemu vam nije odobren. Bićete naknadno obaviješteni mejlom kada vam se nalog odobri",
				ErrorCode.UserNotApproved.ToString()
			);

		await CheckUserPasswordAsync(user, password);

		user.AuthToken = await jwtService.GenerateJwtTokenAsync(user);


        RoleName roleName = Enum.Parse<RoleName>(user.Roles.FirstOrDefault()?.Role.Name ?? throw new ApiException("Missing role"));
		user.Permissions = PermissionClaim.GetDefaultRolePermissions(roleName);


		await userRepository.SaveLastLoginAsync(user, DateTime.UtcNow);

		return user;
	}

    private async Task CheckUserPasswordAsync(User user, string password)
    {
        var checkPwd = await signInManager.CheckPasswordSignInAsync(
            user,
            password,
            true
        );

        if (checkPwd.Succeeded)
            return;

        if (checkPwd.IsLockedOut && user.LockoutEnd.HasValue)
        {
            if (
                user.LockoutEnd.Value.ToUniversalTime()
                >= DateTime.UtcNow.AddDays(10).ToUniversalTime()
            )
                throw new ApiException(
                    "Vaš nalog je zaključan. Obratite se administratoru sistema za pomoć.",
                    ErrorCode.AccountLocked.ToString()
                );

            throw new ApiException(
                $"Dostigli ste maksimalan broj pokušaja prijavljivanja. Čekajte do {user.LockoutEnd.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss} za sljedeći pokušaj.",
                ErrorCode.AccountLocked.ToString()
            );
        }
        if (checkPwd.IsNotAllowed)
            throw new ApiException(
                "Nije vam dozvoljeno da se prijavite.",
                ErrorCode.LoginNotAllowed.ToString()
            );

        throw new ApiException(
            "Korisničko ime ili lozinka su pogrešni.",
            ErrorCode.InvalidCredentials.ToString()
        );
    }
}
