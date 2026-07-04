using Microsoft.EntityFrameworkCore;
using Supermarket.App.Algorithms;
using Supermarket.App.Data;
using Supermarket.App.DataStructures;
using Supermarket.App.Models;

namespace Supermarket.App.Services;

// Holds the result of a SmartSearch - which algorithm ran and what it found
public class SearchResult
{
    public string Method = "";
    public List<Product> Results = new();
}

// Product and stock operations
// Saving goes through EF but all searching runs on the custom data structures
public class ProductServ
{
    private readonly SupermrktCont _db;
    public int LowStockThreshold { get; set; } = 5;

    public ProductServ(SupermrktCont db) => _db = db;

    // Loads every product from the database into the custom linked list
    public CstmLinkedList<Product> LoadAll()
    {
        var list = new CstmLinkedList<Product>();
        foreach (var p in _db.Products.Include(p => p.Category).Include(p => p.Supplier).AsNoTracking())
            list.Add(p);
        return list;
    }

    // Builds a BST of products keyed by name for fast exact-name search
    public BST<string, Product> BuildNameTree(CstmLinkedList<Product> products)
    {
        var tree = new BST<string, Product>();
        foreach (var p in products) tree.Insert(p.Name.ToLowerInvariant(), p);
        return tree;
    }

    // Builds a hash table of products keyed by barcode
    public Hashtable<string, Product> BuildBarcodeIndex(CstmLinkedList<Product> products)
    {
        var table = new Hashtable<string, Product>();
        foreach (var p in products) table.Put(p.Barcode, p);
        return table;
    }

    // Exact name search using the BST, O(log n) average
    public CstmLinkedList<Product> SearchByNameExact(BST<string, Product> tree, string name)
        => tree.Search(name.ToLowerInvariant());

    // Partial name search using a linear scan, O(n)
    public CstmLinkedList<Product> SearchByNameContains(CstmLinkedList<Product> products, string term)
        => SearchAlgo.LinearSearch(products, p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase));

    // Barcode lookup using the hash table, O(1) average
    public Product? SearchByBar(Hashtable<string, Product> index, string barcode)
        => index.TryGet(barcode, out var p) ? p : null;

    // Sorts the products with quicksort then binary searches them by name
    public Product? BinarySearchByName(CstmLinkedList<Product> products, string name)
    {
        var arr = products.ToArray();
        SearchAlgo.QuickSort(arr);
        int i = SearchAlgo.BinarySearch(arr, name);
        return i >= 0 ? arr[i] : null;
    }

    // Finds all products in the given category
    public CstmLinkedList<Product> SearchByCategory(CstmLinkedList<Product> products, string category)
        => SearchAlgo.LinearSearch(products, p => p.Category?.Name.Equals(category, StringComparison.OrdinalIgnoreCase) == true);

    // Finds all products from the given supplier
    public CstmLinkedList<Product> SearchBySupplier(CstmLinkedList<Product> products, string supplier)
        => SearchAlgo.LinearSearch(products, p => p.Supplier?.Name.Equals(supplier, StringComparison.OrdinalIgnoreCase) == true);

    // Picks the right search algorithm from the term itself
    // digits only -> barcode hash lookup, exact name -> BST, anything else -> linear scan
    public SearchResult SmartSearch(string term)
    {
        var all = LoadAll();
        term = term.Trim();
        var result = new SearchResult();

        if (term.Length > 0 && term.All(char.IsDigit))
        {
            result.Method = "Barcode → hash table, O(1) average";
            var hit = SearchByBar(BuildBarcodeIndex(all), term);
            if (hit != null) result.Results.Add(hit);
            return result;
        }

        var exact = SearchByNameExact(BuildNameTree(all), term).ToList();
        if (exact.Count > 0)
        {
            result.Method = "Exact name → binary search tree, O(log n) average";
            result.Results = exact;
            return result;
        }

        // nothing matched exactly so fall back to a scan over name, category and supplier
        result.Method = "Name/category/supplier → linear search, O(n)";
        result.Results = SearchAlgo.LinearSearch(all, p =>
            p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            p.Category?.Name.Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
            p.Supplier?.Name.Contains(term, StringComparison.OrdinalIgnoreCase) == true).ToList();
        return result;
    }

    // Adds a new product after validating it
    public Product Add(Product p)
    {
        Validate(p);
        _db.Products.Add(p);
        _db.SaveChanges();
        return p;
    }

    // Updates an existing product after validating the changes
    public void Update(Product p)
    {
        Validate(p);
        _db.Products.Update(p);
        _db.SaveChanges();
    }

    // Deletes a product, returns false if the id does not exist
    public bool Delete(int id)
    {
        var p = _db.Products.Find(id);
        if (p is null) return false;
        _db.Products.Remove(p);
        _db.SaveChanges();
        return true;
    }

    // Gets one product with its category and supplier loaded
    public Product? Get(int id) =>
        _db.Products.Include(p => p.Category).Include(p => p.Supplier).FirstOrDefault(p => p.Id == id);

    // Increases or decreases stock and logs the movement, refuses to go below zero
    public void AdjustStock(int productId, int change, string reason)
    {
        var p = _db.Products.Find(productId) ?? throw new InvalidOperationException("Product not found.");
        if (p.Quantity + change < 0) throw new InvalidOperationException("Stock cannot go negative.");
        p.Quantity += change;
        if (change > 0) p.RestockDate = DateTime.Today;
        _db.StockMovements.Add(new StockMovement { ProductId = productId, Change = change, Reason = reason });
        _db.SaveChanges();
    }

    // All the validation rules for a product, throws with a clear message when one fails
    private void Validate(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(p.Barcode)) throw new ArgumentException("Barcode is required.");
        if (p.Price <= 0) throw new ArgumentException("Price must be positive.");
        if (p.Quantity < 0) throw new ArgumentException("Quantity cannot be negative.");
        if (p.SupplierId <= 0) throw new ArgumentException("A supplier is required.");
        if (p.CategoryId <= 0) throw new ArgumentException("A category is required.");
        if (p.ExpiryDate.HasValue && p.ExpiryDate.Value.Date < DateTime.Today) throw new ArgumentException("Expiry date cannot be in the past.");

        // the same product keeps its own barcode on update, anyone else using it is a duplicate
        bool barcodeTaken = _db.Products.Any(x => x.Barcode == p.Barcode && x.Id != p.Id);
        if (barcodeTaken) throw new ArgumentException($"Barcode '{p.Barcode}' already exists.");
    }
}
