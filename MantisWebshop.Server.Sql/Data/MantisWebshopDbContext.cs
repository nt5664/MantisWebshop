using MantisWebshop.Server.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace MantisWebshop.Server.Sql.Data
{
    public class MantisWebshopDbContext : DbContext
    {
        public MantisWebshopDbContext(DbContextOptions<MantisWebshopDbContext> options) : base(options) 
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSnapshot> ProductSnapshots { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<ProductSnapshot>().ToTable("productSnapshots");
            modelBuilder.Entity<Order>().ToTable("order");
            modelBuilder.Entity<CartItem>().ToTable("cartItems");
        }
    }
}
