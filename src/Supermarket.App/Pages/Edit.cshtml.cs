using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class EditModel : PageModel
{
    private readonly ProductServ _products;
    private readonly CatalogServ _catalog;

    public EditModel(ProductServ products, CatalogServ catalog) { _products = products; _catalog = catalog; }

    public List<Category> Categories = new();
    public List<Supplier> Suppliers = new();
    [BindProperty] public ProductInput Input { get; set; } = new();
    public string? Error { get; set; }

    public class ProductInput
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Brand { get; set; }
        public string Barcode { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public IActionResult OnGet(int id)
    {
        var p = _products.Get(id);
        if (p is null) return RedirectToPage("/Products");
        Input = new ProductInput
        {
            Id = p.Id, Name = p.Name, Brand = p.Brand, Barcode = p.Barcode,
            Price = p.Price, Quantity = p.Quantity, CategoryId = p.CategoryId,
            SupplierId = p.SupplierId, ExpiryDate = p.ExpiryDate
        };
        Load();
        return Page();
    }

    public IActionResult OnPost()
    {
        try
        {
            _products.Update(new Product
            {
                Id = Input.Id, Name = Input.Name, Brand = Input.Brand, Barcode = Input.Barcode,
                Price = Input.Price, Quantity = Input.Quantity, CategoryId = Input.CategoryId,
                SupplierId = Input.SupplierId, ExpiryDate = Input.ExpiryDate
            });
            return RedirectToPage("/Products");
        }
        catch (Exception ex) { Error = ex.Message; Load(); return Page(); }
    }

    private void Load() { Categories = _catalog.Categories(); Suppliers = _catalog.Suppliers(); }
}
