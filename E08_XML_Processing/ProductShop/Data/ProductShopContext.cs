namespace ProductShop.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
        {

        }

        public ProductShopContext(DbContextOptions options)
            : base(options)
        {

        }

        public virtual DbSet<Product> Products { get; set; } = null!;

        public virtual DbSet<Category> Categories { get; set; } = null!;

        public virtual DbSet<User> Users { get; set; } = null!;

        public virtual DbSet<CategoryProduct> CategoryProducts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity
                    .HasKey(x => new { x.CategoryId, x.ProductId });
            });

            // Required to configure the multiple relationships between User and Product
            // Alt: Use [InverseProperty] attribute in the Product class
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(u => u.ProductsBought)
                      .WithOne(p => p.Buyer)
                      .HasForeignKey(p => p.BuyerId);

                entity.HasMany(u => u.ProductsSold)
                      .WithOne(p => p.Seller)
                      .HasForeignKey(p => p.SellerId);
            });
        }
    }
}