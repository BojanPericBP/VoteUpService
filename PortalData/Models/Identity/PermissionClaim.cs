using VoteUp.PortalData.Models.Identity.Constants;

namespace VoteUp.PortalData.Models.Identity;

public class PermissionClaim
{
	// Use this method to define set of permissions for particular roles
	public static List<string> GetDefaultRolePermissions(RoleName role)
	{
		return role switch
		{
			RoleName.SuperAdmin
				=>
				[
					Permission.AllUsersManage,
					Permission.ReadCity,
					Permission.CreateCity,
					Permission.UpdateCity,
					Permission.DeleteCity,
					Permission.RestoreCity
				],
			RoleName.VotingOperator
				=>
				[
					Permission.ReadCity,
					Permission.CreateCity,
					Permission.UpdateCity,
					Permission.DeleteCity,
					Permission.RestoreCity
				],
			_ => throw new Exception("Invalid role provided"),
		};
	}
}
