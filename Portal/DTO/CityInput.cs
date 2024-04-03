using VoteUp.Portal.Exceptions;
using VoteUp.PortalData.Models.Base;

namespace VoteUp.Portal.DTO;

public record CityInput(Guid? Id, string Name, int? Order, string? Description)
{
	public City MapToCreate() =>
		new()
		{
			Name = Name,
			Order = Order ?? 100,
			Description = Description
		};

	public dynamic MapToUpdate() =>
		new
		{
			Id = Id ?? throw new ApiException("Missing Id"),
            Name,
            Order = Order ?? 100,
            Description
        };
}
