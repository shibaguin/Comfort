using Microsoft.EntityFrameworkCore;

namespace Comfort.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<ProductWorkshop> ProductWorkshops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка уникальных индексов
        modelBuilder.Entity<ProductType>()
            .HasIndex(pt => pt.ProductTypeName)
            .IsUnique();

        modelBuilder.Entity<Material>()
            .HasIndex(m => m.MaterialName)
            .IsUnique();

        modelBuilder.Entity<Workshop>()
            .HasIndex(w => w.WorkshopName)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Article)
            .IsUnique();
    }
} 