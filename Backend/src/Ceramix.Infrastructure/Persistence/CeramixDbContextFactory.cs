using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ceramix.Infrastructure.Persistence;

public class CeramixDbContextFactory : IDesignTimeDbContextFactory<CeramixDbContext>
{
    public CeramixDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CeramixDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=MSSQLLocalDB;Database=CeramixDB;Trusted_Connection=True;TrustServerCertificate=True;"
        );

        return new CeramixDbContext(optionsBuilder.Options);
    }
}