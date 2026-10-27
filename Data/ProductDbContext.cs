using Microsoft.EntityFrameworkCore;
using MyFirstApi_cs.Models;

namespace MyFirstApi_cs.Data;

public sealed class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).HasMaxLength(200).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(2000);
            entity.Property(product => product.Price).HasPrecision(10, 2);
            entity.Property(product => product.CreatedAt).IsRequired();
            entity.Property(product => product.UpdatedAt).IsRequired();
        });
    }
}