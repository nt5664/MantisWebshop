using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MantisWebshop.Server.Sql.Data
{
    public class MantisWebshopDbContextFactory : IDesignTimeDbContextFactory<MantisWebshopDbContext>
    {
        public MantisWebshopDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MantisWebshopDbContext>();
            builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MantisShopDatabase;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new MantisWebshopDbContext(builder.Options);
        }
    }
}
