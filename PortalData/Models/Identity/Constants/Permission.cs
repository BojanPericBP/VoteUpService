using System.Reflection;

namespace VoteUp.PortalData.Models.Identity.Constants;

public static partial class Permission
{
	public const string AllUsersManage = "AllUsersManage";

	// City
	public const string ReadCity = "ReadCity";
	public const string CreateCity = "CreateCity";
	public const string UpdateCity = "UpdateCity";
	public const string DeleteCity = "DeleteCity";
	public const string RestoreCity = "RestoreCity";
	
}


public static partial class Permission
{
	public static HashSet<string> GetAll()
	{
		return typeof(Permission)
			.GetFields(BindingFlags.Public | BindingFlags.Static)
			.Select(f => (string)f.GetValue(null)!)
			.ToHashSet();
	}
}

