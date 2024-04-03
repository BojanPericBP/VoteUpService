using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VoteUp.PortalData;

public class VoteUpDbContextDesignTimeFactory : IDesignTimeDbContextFactory<VoteUpDbContext>
{
    public VoteUpDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VoteUpDbContext>();

        optionsBuilder.UseNpgsql().UseSnakeCaseNamingConvention();
    
        return new VoteUpDbContext(optionsBuilder.Options);
    }
}

// Use dotnet ef database update --connection "Host=<host>;Port=<port>;Database=<database>;Username=<username>;Password=<password>"

// --connection "Host=localhost;Port=65432;Database=voteup;Username=voteup;Password=voteup123"
