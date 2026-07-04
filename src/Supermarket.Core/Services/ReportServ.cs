using Microsoft.EntityFrameworkCore;
using Supermarket.App.Data;
using Supermarket.App.Models;

namespace Supermarket.App.Services;

// Generates the four reports the shop needs
public class ReportServ
{
    private readonly SupermrktCont _db;
    public ReportServ(SupermrktCont db) => _db = db;

    // Products at or below the threshold, lowest stock first
    public List<Product> LowStock(int threshold = 5) =>
        _db.Products.Include(p => p.Supplier).Where(p => p.Quantity <= threshold).OrderBy(p => p.Quantity).ToList();

    // Units sold and revenue per product, biggest earner first
    // AsEnumerable pulls the rows out first because EF cant turn this grouping into SQL
    public List<(string Product, int Units, decimal Revenue)> SalesByProduct() =>
        _db.SaleItems.Include(i => i.Product).AsEnumerable()
            .GroupBy(i => i.Product!.Name)
            .Select(g => (g.Key, g.Sum(i => i.Quantity), g.Sum(i => i.Quantity * i.UnitPrice)))
            .OrderByDescending(t => t.Item3).ToList();

    // Product count and total units held per category
    public List<(string Category, int Products, int Units)> ProductsByCategory() =>
        _db.Products.Include(p => p.Category).AsEnumerable()
            .GroupBy(p => p.Category!.Name)
            .Select(g => (g.Key, g.Count(), g.Sum(p => p.Quantity)))
            .OrderBy(t => t.Item1).ToList();

    // Product count and total units held per supplier
    public List<(string Supplier, int Products, int Units)> SupplierStock() =>
        _db.Products.Include(p => p.Supplier).AsEnumerable()
            .GroupBy(p => p.Supplier!.Name)
            .Select(g => (g.Key, g.Count(), g.Sum(p => p.Quantity)))
            .OrderBy(t => t.Item1).ToList();
}
