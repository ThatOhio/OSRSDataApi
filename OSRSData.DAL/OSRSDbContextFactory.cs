using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OSRSData.DAL;

public class OSRSDbContextFactory : IDesignTimeDbContextFactory<OSRSDbContext>
{
    public OSRSDbContext CreateDbContext(string[] args)
    {
        // This factory is used by Entity Framework Core tools at design time.
        // It allows creating migrations even when the startup project (OSRSData.Api) 
        // cannot be run in the current environment (e.g., due to missing frameworks).
        
        var optionsBuilder = new DbContextOptionsBuilder<OSRSDbContext>();
        
        // We use localhost here because the tools run on the host machine,
        // while the database is running in a Docker container with port 5432 mapped.
        optionsBuilder.UseNpgsql("Host=localhost;Database=OSRSData;Username=postgres;Password=postgres");

        return new OSRSDbContext(optionsBuilder.Options);
    }
}
