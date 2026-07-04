using Supermarket.App.Data;
using Supermarket.App.Models;

namespace Supermarket.App.Services;

// Add, edit and delete for categories and suppliers
public class CatalogServ
{
    private readonly SupermrktCont _db;
    public CatalogServ(SupermrktCont db) => _db = db;

    // Returns all categories ordered by id
    public List<Category> Categories() => _db.Categories.OrderBy(c => c.Id).ToList();

    // Adds a new category, rejects blanks and duplicates
    public Category AddCategory(string name)
    {
        Require(name, "Category name");
        if (_db.Categories.Any(c => c.Name == name)) throw new ArgumentException("Category already exists.");
        var c = new Category { Name = name };
        _db.Categories.Add(c); _db.SaveChanges();
        return c;
    }

    // Renames an existing category
    public void RenameCategory(int id, string name)
    {
        Require(name, "Category name");
        var c = _db.Categories.Find(id) ?? throw new InvalidOperationException("Category not found.");
        c.Name = name; _db.SaveChanges();
    }

    // Deletes a category as long as no products still use it
    public bool DeleteCategory(int id)
    {
        if (_db.Products.Any(p => p.CategoryId == id)) throw new InvalidOperationException("Category has products.");
        var c = _db.Categories.Find(id);
        if (c is null) return false;
        _db.Categories.Remove(c); _db.SaveChanges();
        return true;
    }

    // Returns all suppliers ordered by id
    public List<Supplier> Suppliers() => _db.Suppliers.OrderBy(s => s.Id).ToList();

    // Adds a new supplier
    public Supplier AddSupplier(string name, string? contact)
    {
        Require(name, "Supplier name");
        var s = new Supplier { Name = name, Contact = contact };
        _db.Suppliers.Add(s); _db.SaveChanges();
        return s;
    }

    // Updates a supplier's name and contact details
    public void UpdateSupplier(int id, string name, string? contact)
    {
        Require(name, "Supplier name");
        var s = _db.Suppliers.Find(id) ?? throw new InvalidOperationException("Supplier not found.");
        s.Name = name; s.Contact = contact; _db.SaveChanges();
    }

    // Deletes a supplier as long as no products still use it
    public bool DeleteSupplier(int id)
    {
        if (_db.Products.Any(p => p.SupplierId == id)) throw new InvalidOperationException("Supplier has products.");
        var s = _db.Suppliers.Find(id);
        if (s is null) return false;
        _db.Suppliers.Remove(s); _db.SaveChanges();
        return true;
    }

    private static void Require(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{field} is required.");
    }
}
