using VoteUp.PortalData.Models.Identity;

namespace VoteUp.Portal.GQL.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);

        descriptor.Field(x => x.FirstName);
        descriptor.Field(x => x.LastName);
        descriptor.Field(x => x.Email);
        descriptor.Field(x => x.EmailConfirmed);
        descriptor.Field(x => x.LastLogin);

        descriptor.Field(x => x.Created);
        descriptor.Field(x => x.Modified);
        descriptor.Field(x => x.Deleted);

        descriptor.Field(x => x.LastLogin);
        descriptor.Field(x => x.AuthToken);

        descriptor
            .Field("permissions")
            .Resolve(context =>
            {
                var user = context.Parent<User>();
                return user.Permissions ?? [];
            });

        descriptor
            .Field("roles")
            .Resolve(
                context =>
                    context.Parent<User>()?.Roles?.Select(x => x.Role.Name).ToList()
            );
    }
}
