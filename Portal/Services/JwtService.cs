using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VoteUp.Portal.Util;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.Portal.Services;

public interface IJwtService
{
    Task<string> GenerateJwtTokenAsync(User user);
}

public class JwtService(
    UserManager<User> userManager,
    IOptions<JwtTokenSettings> tokenSettings
    ) : IJwtService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly JwtTokenSettings _tokenSettings = tokenSettings.Value;

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        foreach (var r in roles)
            if (await _userManager.IsInRoleAsync(user, r.ToString()))
            {
                claims.Add(new Claim(ClaimTypes.Role, r.ToString()));
                foreach (string permission in PermissionClaim.GetDefaultRolePermissions(Enum.Parse<RoleName>(r)))
                    claims.Add(new Claim(CustomClaimTypes.Permission, permission));
            }

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()));
        claims.Add(
            new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}")
        );

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        int ttl = _tokenSettings.Ttl == 0 ? 24 : _tokenSettings.Ttl;

        DateTime expiresDateTime = DateTime.UtcNow.AddHours(ttl);

        var secToken = new JwtSecurityToken(
            _tokenSettings.Issuer,
            _tokenSettings.Audience,
            claims,
            expires: expiresDateTime,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(secToken);
    }
}
