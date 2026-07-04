using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.DataStructures;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class SellModel : PageModel
{
    private readonly ProductServ _products;
    private readonly SalesServ _sales;

    public SellModel(ProductServ products, SalesServ sales) { _products = products; _sales = sales; }

    public CstmLinkedList<Product> All = new();
    public Sale? Receipt;
    public string? Error;

    public void OnGet() => Load();

    // Up to five lines posted as parallel arrays productId[] / quantity[].
    public IActionResult OnPost(int[] productId, int[] quantity)
    {
        var lines = new List<(int, int)>();
        for (int i = 0; i < productId.Length; i++)
            if (productId[i] > 0 && i < quantity.Length && quantity[i] > 0)
                lines.Add((productId[i], quantity[i]));

        Load();
        if (lines.Count == 0) { Error = "Select at least one product and quantity."; return Page(); }

        try { Receipt = _sales.Record(lines); }
        catch (Exception ex) { Error = ex.Message; }
        return Page();
    }

    private void Load() => All = _products.LoadAll();

    public string ProductName(int id) => _products.Get(id)?.Name ?? $"#{id}";
}
