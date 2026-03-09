using ECommerce.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                    new Product
                    {
                        Id = 3, Name = "iPhone 16 Pro", Description = "256Go", Price = 1199.99m, ImageUrl = "ihpone.jpg", Stock = 10
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "MacBook Air",
                        Description = "M3 16Go",
                        Price = 1299.99m,
                        ImageUrl = "macbook.jpg",
                        Stock = 5
                    }
                );
        }
    }
}
