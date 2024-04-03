using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VoteUp.Portal.Util;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.Portal.Extensions;

public static class HttpContextAccessorExstensions
{
	public static ClaimsPrincipal? GetCurrentUser(this IHttpContextAccessor httpContext)
	{
		return httpContext.HttpContext?.User;
	}

	public static Guid? GetCurrentUserId(this IHttpContextAccessor httpContext)
	{
		var userIdClaim = GetCurrentUser(httpContext)?.FindFirst(JwtRegisteredClaimNames.Jti);

		if (userIdClaim != null)
			return Guid.TryParse(userIdClaim.Value, out Guid userId) ? userId : null;

		return null;
	}

	public static string? GetCurrentClientRemoteIp(this IHttpContextAccessor httpContext)
	{
		var ipAddress = httpContext?.HttpContext?.Connection.RemoteIpAddress;

		if (ipAddress == null)
			return null;

		if (ipAddress.IsIPv4MappedToIPv6)
			ipAddress = ipAddress.MapToIPv4();

		return ipAddress.ToString();
	}

	public static List<RoleName> GetCurrentUserRoles(this IHttpContextAccessor httpContext)
	{
		ClaimsPrincipal? user = GetCurrentUser(httpContext);

		if (user?.Identity is null)
			return [];

		var userRoles = ((ClaimsIdentity)user.Identity)
			.Claims.Where(c => c.Type == ClaimTypes.Role)
			.Select(c => c.Value)
			.ToList();

		return userRoles.ConvertAll(x => (RoleName)Enum.Parse(typeof(RoleName), x));
	}

	public static List<string> GetCurrentUserPermissions(this IHttpContextAccessor httpContext)
	{
		ClaimsPrincipal? user = GetCurrentUser(httpContext);

		if (user?.Identity is null)
			return [];

		return ((ClaimsIdentity)user.Identity)
			.Claims.Where(c => c.Type == "permission")
			.Select(c => c.Value)
			.ToList();
	}

	public static bool IsInRole(this IHttpContextAccessor httpContext, RoleName roleName)
	{
		bool isInRole = false;
		List<RoleName> roles = GetCurrentUserRoles(httpContext);
		if (roles != null)
			isInRole = roles.Any(x => x == roleName);

		return isInRole;
	}

	public static bool HasPermission(this IHttpContextAccessor httpContext, string permission)
	{
		bool hasPermission = false;
		List<string> permissions = GetCurrentUserPermissions(httpContext);
		if (permissions != null)
			hasPermission = permissions.Any(x => x == permission);

		return hasPermission;
	}

	// public static int? GetCurrentLanguageId(this IHttpContextAccessor httpContext)
	// {
	//     int? languageId = null;
	//     var user = GetCurrentUser(httpContext);

	//     var languageIdClaim = user?.FindFirst("LanguageId");

	//     if (languageIdClaim != null && !string.IsNullOrEmpty(languageIdClaim.Value))
	//         languageId = int.Parse(languageIdClaim.Value);

	//     return languageId;
	// }

    public static AuthContext MapToAuthContext(this IHttpContextAccessor httpContext)
	{
		return new(){
            UserId = httpContext.GetCurrentUserId(),
            RemoteIpAddress = httpContext.GetCurrentClientRemoteIp(),
            Roles = httpContext.GetCurrentUserRoles(),
            Permissions = httpContext.GetCurrentUserPermissions()
        };
	}
}
