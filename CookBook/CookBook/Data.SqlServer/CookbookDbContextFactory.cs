using Data.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CookBook.Data.SqlServer;

public class CookBookDbContextFactory : IDesignTimeDbContextFactory<CookBookDbContext>
{
    public CookBookDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.database.json")
            .Build();

        return CreateDbContext(configuration);
    }

    public CookBookDbContext CreateDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<CookBookDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CookBookDbContext(optionsBuilder.Options);
    }
}
