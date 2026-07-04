using Microsoft.EntityFrameworkCore;
using Supermarket.App.Data;
using Supermarket.App.Models;

namespace Supermarket.App.Services;

// Records sales, reduces stock and keeps the sale history
public class SalesServ
{
    private readonly SupermrktCont _db;
    public SalesServ(SupermrktCont db) => _db = db;

    // Records a sale from (productId, quantity) lines
    // Nothing is saved until the end, so if any line fails the whole sale is cancelled
    public Sale Record(IEnumerable<(int productId, int quantity)> lines)
    {
        var sale = new Sale();
        foreach (var (productId, qty) in lines)
        {
            if (qty <= 0) throw new ArgumentException("Quantity must be positive.");
            var p = _db.Products.Find(productId) ?? throw new InvalidOperationException($"Product {productId} not found.");
            if (p.Quantity < qty) throw new InvalidOperationException($"Not enough stock for '{p.Name}' (have {p.Quantity}).");

            // stock drops automatically when the product is sold
            p.Quantity -= qty;
            sale.Items.Add(new SaleItem { ProductId = p.Id, Quantity = qty, UnitPrice = p.Price });
            _db.StockMovements.Add(new StockMovement { ProductId = p.Id, Change = -qty, Reason = "Sale" });
        }
        if (sale.Items.Count == 0) throw new ArgumentException("A sale needs at least one item.");

        sale.Total = sale.Items.Sum(i => i.LineTotal);
        _db.Sales.Add(sale);
        _db.SaveChanges();
        return sale;
    }

    // Returns every sale with its items, newest first
    public List<Sale> History() =>
        _db.Sales.Include(s => s.Items).ThenInclude(i => i.Product).OrderByDescending(s => s.Date).ToList();
}
