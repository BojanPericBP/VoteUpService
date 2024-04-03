using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoteUp.PortalData.Models.Identity;

namespace VoteUp.PortalData.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");
	}
}

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.ToTable("roles");
	}
}

internal sealed class RoleClaimsConfiguration : IEntityTypeConfiguration<RoleClaims>
{
	public void Configure(EntityTypeBuilder<RoleClaims> builder)
	{
		builder.ToTable("role_claims");
		builder
			.HasOne(roleClaim => roleClaim.Role)
			.WithMany(role => role.Claims)
			.HasForeignKey(roleClaim => roleClaim.RoleId);
	}
}

internal sealed class UserClaimsConfiguration : IEntityTypeConfiguration<UserClaims>
{
	public void Configure(EntityTypeBuilder<UserClaims> builder)
	{
		builder.ToTable("user_claims");
		builder
			.HasOne(userClaim => userClaim.User)
			.WithMany(user => user.Claims)
			.HasForeignKey(userClaim => userClaim.UserId);

		builder.HasQueryFilter(p => p.User.Deleted == null); // User has query filter and it must be defined on both sides of required navigation
	}
}

internal sealed class UserLoginsConfiguration : IEntityTypeConfiguration<UserLogins>
{
	public void Configure(EntityTypeBuilder<UserLogins> builder)
	{
		builder.ToTable("user_logins");
		builder
			.HasOne(userLogin => userLogin.User)
			.WithMany(user => user.Logins)
			.HasForeignKey(userLogin => userLogin.UserId);

		builder.HasQueryFilter(p => p.User.Deleted == null);
	}
}

internal sealed class UserRolesConfiguration : IEntityTypeConfiguration<UserRoles>
{
	public void Configure(EntityTypeBuilder<UserRoles> builder)
	{
		builder.ToTable("user_roles");
		builder
			.HasOne(userRole => userRole.Role)
			.WithMany(role => role.Users)
			.HasForeignKey(userRole => userRole.RoleId);
		builder
			.HasOne(userRole => userRole.User)
			.WithMany(user => user.Roles)
			.HasForeignKey(userRole => userRole.UserId);

		builder.HasQueryFilter(p => p.User.Deleted == null);
	}
}

internal sealed class UserTokensConfiguration : IEntityTypeConfiguration<UserTokens>
{
	public void Configure(EntityTypeBuilder<UserTokens> builder)
	{
		builder.ToTable("user_tokens");
		builder
			.HasOne(userToken => userToken.User)
			.WithMany(user => user.UserTokens)
			.HasForeignKey(userToken => userToken.UserId);

		builder.HasQueryFilter(p => p.User.Deleted == null);
	}
}
