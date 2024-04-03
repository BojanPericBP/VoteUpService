namespace VoteUp.PortalData.Models.Interfaces;

public interface IIdentity<T>
{
	T Id { get; set; }
}
