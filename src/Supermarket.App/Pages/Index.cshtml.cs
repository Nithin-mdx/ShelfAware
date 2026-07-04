using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.Data;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class IndexModel : PageModel
{
    private readonly SupermrktCont _db;
    private readonly ReportServ _reports;

    public IndexModel(SupermrktCont db, ReportServ reports) { _db = db; _reports = reports; }

    public int ProductCount, SupplierCount, SaleCount;
    public List<Product> LowStock = new();

    public void OnGet()
    {
        ProductCount = _db.Products.Count();
        SupplierCount = _db.Suppliers.Count();
        SaleCount = _db.Sales.Count();
        LowStock = _reports.LowStock();
    }
}
