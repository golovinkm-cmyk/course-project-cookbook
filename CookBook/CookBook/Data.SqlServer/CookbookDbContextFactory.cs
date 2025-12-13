using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Data.SqlServer
{
    public class CookbookDbContextFactory : IDesignTimeDbContextFactory<CookbookDbContext>
    {
        public CookbookDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.database.json")
                .Build();

            return CreateDbContext(configuration);
        }

        public CookbookDbContext CreateDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<CookbookDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new CookbookDbContext(optionsBuilder.Options);
        }
    }
}
