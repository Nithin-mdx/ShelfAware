using Microsoft.EntityFrameworkCore;
using Supermarket.App.Models;

namespace Supermarket.App.Data;

public class SupermrktCont : DbContext
{
    // Default local connection. Override by passing options (tests use the in-memory provider).
    public const string DefaultConnection =
        @"Server=.\SQLEXPRESS;Database=SupermarketDb;Trusted_Connection=True;TrustServerCertificate=True";

    public SupermrktCont() { }
    public SupermrktCont(DbContextOptions<SupermrktCont> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured) options.UseSqlServer(DefaultConnection);
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Unique index means the database itself rejects duplicate barcodes
        b.Entity<Product>().HasIndex(p => p.Barcode).IsUnique();
        b.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(10,2)");
        b.Entity<Sale>().Property(s => s.Total).HasColumnType("decimal(10,2)");
        b.Entity<SaleItem>().Property(s => s.UnitPrice).HasColumnType("decimal(10,2)");

        // Restrict stops a category or supplier being deleted while products still use it
        b.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Product>().HasOne(p => p.Supplier).WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
}
