using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class ReportsModel : PageModel
{
    private readonly ReportServ _reports;
    public ReportsModel(ReportServ reports) => _reports = reports;

    public List<Product> LowStock = new();
    public List<(string Product, int Units, decimal Revenue)> SalesByProduct = new();
    public List<(string Category, int Products, int Units)> ByCategory = new();
    public List<(string Supplier, int Products, int Units)> SupplierStock = new();

    public void OnGet()
    {
        LowStock = _reports.LowStock();
        SalesByProduct = _reports.SalesByProduct();
        ByCategory = _reports.ProductsByCategory();
        SupplierStock = _reports.SupplierStock();
    }
}
