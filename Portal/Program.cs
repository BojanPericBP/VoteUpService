using VoteUp.Portal.Exceptions;
using VoteUp.Portal.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddAuthContext()
    .AddVoteUpDb(builder.Configuration, builder.Environment)
    .AddIdentity(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddDefaultAuthorization()
    .AddRepositories()
    .AddServices()
    .AddSendgrid(builder.Configuration)
    .ConfigureCors()
    .ConfigureGraphQL();
    

var app = builder.Build();

app.MigrateDb();

if(builder.Environment.IsProduction())
    app.UseCors("prod");
else
    app.UseCors("dev");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.MapGraphQL();

app.Run();
