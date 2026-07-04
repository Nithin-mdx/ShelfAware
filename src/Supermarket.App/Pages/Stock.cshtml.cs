using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.DataStructures;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class StockModel : PageModel
{
    private readonly ProductServ _products;
    private readonly ReportServ _reports;

    public StockModel(ProductServ products, ReportServ reports) { _products = products; _reports = reports; }

    public CstmLinkedList<Product> All = new();
    public List<Product> LowStock = new();
    [TempData] public string? Message { get; set; }
    public string? Error { get; set; }

    public void OnGet() => Load();

    public IActionResult OnPostRestock(int productId, int amount)
    {
        try { _products.AdjustStock(productId, amount, "Restock"); Message = "Stock increased."; return RedirectToPage(); }
        catch (Exception ex) { Error = ex.Message; Load(); return Page(); }
    }

    public IActionResult OnPostReduce(int productId, int amount)
    {
        try { _products.AdjustStock(productId, -amount, "Adjustment"); Message = "Stock reduced."; return RedirectToPage(); }
        catch (Exception ex) { Error = ex.Message; Load(); return Page(); }
    }

    private void Load() { All = _products.LoadAll(); LowStock = _reports.LowStock(); }
}
