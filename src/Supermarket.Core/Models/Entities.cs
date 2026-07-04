using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.App.Models;

// All the entity classes, one per database table

public class Category
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Supplier
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(100)] public string? Contact { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public int Id { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = "";       // Title
    [MaxLength(100)] public string? Brand { get; set; }
    [Required, MaxLength(50)] public string Barcode { get; set; } = "";     // unique
    public decimal Price { get; set; }
    public int Quantity { get; set; }                                       // quantity in stock
    public DateTime? ExpiryDate { get; set; }
    public DateTime? RestockDate { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    // Stock availability status, derived from quantity (LowStockThreshold default 5).
    [NotMapped] public bool InStock => Quantity > 0;
    public bool IsLowStock(int threshold = 5) => Quantity <= threshold;
}

// Stock table: an audit log of every stock change (restock / sale / adjustment).
public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Change { get; set; }                 // +restock, -sale
    [MaxLength(100)] public string Reason { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Now;
}

public class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}

public class SaleItem
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public Sale? Sale { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    [NotMapped] public decimal LineTotal => Quantity * UnitPrice;
}
