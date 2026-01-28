using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

///Add-Migration Init -Project SkyBuy.Data -StartupProject SkyBuy
///Update-database -Project SkyBuy.Data -StartupProject SkyBuy

namespace SkyBuy.Data.Data
{
    public class SkyBuyContextFactory : IDesignTimeDbContextFactory<SkyBuyContext>
    {
        public SkyBuyContext CreateDbContext(string[] args)
        {
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SkyBuyContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DbConnection"));

            return new SkyBuyContext(optionsBuilder.Options);
        }
    }
}