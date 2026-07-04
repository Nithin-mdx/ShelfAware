using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class CatalogModel : PageModel
{
    private readonly CatalogServ _catalog;
    public CatalogModel(CatalogServ catalog) => _catalog = catalog;

    public List<Category> Categories = new();
    public List<Supplier> Suppliers = new();
    [TempData] public string? Message { get; set; }
    public string? Error { get; set; }

    public void OnGet() => Load();

    public IActionResult OnPostAddCategory(string name) => Do(() => { _catalog.AddCategory(name); Message = "Category added."; });
    public IActionResult OnPostDeleteCategory(int id) => Do(() => { _catalog.DeleteCategory(id); Message = "Category deleted."; });
    public IActionResult OnPostRenameCategory(int id, string name) => Do(() => { _catalog.RenameCategory(id, name); Message = "Category updated."; });
    public IActionResult OnPostAddSupplier(string name, string? contact) => Do(() => { _catalog.AddSupplier(name, contact); Message = "Supplier added."; });
    public IActionResult OnPostEditSupplier(int id, string name, string? contact) => Do(() => { _catalog.UpdateSupplier(id, name, contact); Message = "Supplier updated."; });
    public IActionResult OnPostDeleteSupplier(int id) => Do(() => { _catalog.DeleteSupplier(id); Message = "Supplier deleted."; });

    private IActionResult Do(Action action)
    {
        try { action(); return RedirectToPage(); }
        catch (Exception ex) { Error = ex.Message; Load(); return Page(); }
    }

    private void Load() { Categories = _catalog.Categories(); Suppliers = _catalog.Suppliers(); }
}
