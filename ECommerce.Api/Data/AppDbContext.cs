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

        // Nouvelles tables pour détailler les produits
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Theme> Themes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des rélations many-to-many de la bdd
            modelBuilder.Entity<Product>().HasMany(p => p.Authors).WithMany(a => a.Products);
            modelBuilder.Entity<Product>().HasMany(p => p.Categories).WithMany(a => a.Products);
            modelBuilder.Entity<Product>().HasMany(p=>p.Themes).WithMany(a => a.Products);

            // Configuration de la relation many-to-one de la bdd
            modelBuilder.Entity<Product>().HasOne(p => p.Publishers).WithMany(pub => pub.Products).HasForeignKey(p => p.PublisherId);

            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "Hasbro" },
                new Publisher { Id = 2, Name = "MB" }
            );

            modelBuilder.Entity<Product>().HasData(
                    new Product
                    {
                        Id = 3, 
                        Name = "Monopoly Classique", 
                        Description = "Retrouvez le plus célèbre des jeux de société dans une version classique de toute beauté ! Achetez, vendez et construisez pour devenir le plus riche !", 
                        Price = 29.90m, 
                        ImageUrl = "", 
                        Stock = 10,
                        MinPlayers = 2,
                        MaxPlayers = 6,
                        MinAge = 8,
                        GameDuration = 90,
                        PublisherId = 1

                    },
                    new Product
                    {
                        Id = 4,
                        Name = "La Bonne Paye",
                        Description = "Dans la Bonne Paye, gérez votre argent : investissez, économisez et enrichissez-vous ! Attention aux factures et aux imprévus qui pourraient vous ruiner !",
                        Price = 29.90m,
                        ImageUrl = "macbook.jpg",
                        Stock = 5,
                        MinPlayers = 2,
                        MaxPlayers = 6,
                        MinAge = 8,
                        GameDuration = 60,
                        PublisherId = 2,
                    }
                );
        }
    }
}
