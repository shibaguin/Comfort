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

        // Настройка связей
        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductType)
            .WithMany(pt => pt.Products)
            .HasForeignKey(p => p.ProductTypeID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.MainMaterial)
            .WithMany(m => m.Products)
            .HasForeignKey(p => p.MainMaterialID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductWorkshop>()
            .HasOne(pw => pw.Product)
            .WithMany(p => p.ProductWorkshops)
            .HasForeignKey(pw => pw.ProductID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductWorkshop>()
            .HasOne(pw => pw.Workshop)
            .WithMany(w => w.ProductWorkshops)
            .HasForeignKey(pw => pw.WorkshopID)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 