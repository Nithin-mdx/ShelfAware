using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.DataStructures;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class ProductsModel : PageModel
{
    private readonly ProductServ _products;
    private readonly CatalogServ _catalog;

    public ProductsModel(ProductServ products, CatalogServ catalog) { _products = products; _catalog = catalog; }

    public CstmLinkedList<Product> All = new();
    public List<Category> Categories = new();
    public List<Supplier> Suppliers = new();

    [BindProperty] public ProductInput Input { get; set; } = new();
    [TempData] public string? Message { get; set; }
    public string? Error { get; set; }

    public class ProductInput
    {
        public string Name { get; set; } = "";
        public string? Brand { get; set; }
        public string Barcode { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public void OnGet() => Load();

    public IActionResult OnPostAdd()
    {
        try
        {
            _products.Add(new Product
            {
                Name = Input.Name, Brand = Input.Brand, Barcode = Input.Barcode,
                Price = Input.Price, Quantity = Input.Quantity,
                CategoryId = Input.CategoryId, SupplierId = Input.SupplierId, ExpiryDate = Input.ExpiryDate
            });
            Message = $"Added '{Input.Name}'.";
            return RedirectToPage();
        }
        catch (Exception ex) { Error = ex.Message; Load(); return Page(); }
    }

    public IActionResult OnPostDelete(int id)
    {
        _products.Delete(id);
        Message = "Product deleted.";
        return RedirectToPage();
    }

    private void Load()
    {
        All = _products.LoadAll();
        Categories = _catalog.Categories();
        Suppliers = _catalog.Suppliers();
    }
}
