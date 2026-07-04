using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class SearchModel : PageModel
{
    private readonly ProductServ _products;
    public SearchModel(ProductServ products) => _products = products;

    [BindProperty(SupportsGet = true)] public string? Term { get; set; }

    public List<Product> Results = new();
    public List<string> Names = new();   // product names shown as suggestions while typing
    public string? Method;
    public bool Searched;

    public void OnGet()
    {
        // product names for the suggestion dropdown, filtered by the script on the page
        foreach (var p in _products.LoadAll()) Names.Add(p.Name);
        Names.Sort();

        if (string.IsNullOrWhiteSpace(Term)) return;
        Searched = true;
        var found = _products.SmartSearch(Term);
        Method = found.Method;
        Results = found.Results;
    }
}
