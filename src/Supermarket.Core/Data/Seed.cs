using Supermarket.App.Models;

namespace Supermarket.App.Data;

// Inserts sample data on the first run so the app starts with something to show
public static class Seed
{
    public static void EnsureSeeded(SupermrktCont db)
    {
        if (db.Categories.Any()) return;

        var drinks = new Category { Name = "Drinks" };
        var bakery = new Category { Name = "Bakery" };
        var dairy = new Category { Name = "Dairy" };
        var produce = new Category { Name = "Produce" };
        db.Categories.AddRange(drinks, bakery, dairy, produce);

        var metro = new Supplier { Name = "Metro Cash & Carry", Contact = "orders@metro-cc.com" };
        var dmart = new Supplier { Name = "DMART", Contact = "sales@dmart.com" };
        db.Suppliers.AddRange(metro, dmart);
        db.SaveChanges();

        db.Products.AddRange(
            new Product { Name = "Cola 1L", Brand = "FizzCo", Barcode = "1000001", Price = 1.20m, Quantity = 40, Category = drinks, Supplier = dmart, ExpiryDate = DateTime.Today.AddMonths(8) },
            new Product { Name = "Orange Juice 1L", Brand = "Sunny", Barcode = "1000002", Price = 1.80m, Quantity = 3, Category = drinks, Supplier = metro, ExpiryDate = DateTime.Today.AddDays(20) },
            new Product { Name = "White Bread", Brand = "BakeHouse", Barcode = "1000003", Price = 0.95m, Quantity = 25, Category = bakery, Supplier = dmart, ExpiryDate = DateTime.Today.AddDays(5) },
            new Product { Name = "Croissant", Brand = "BakeHouse", Barcode = "1000004", Price = 0.70m, Quantity = 2, Category = bakery, Supplier = dmart, ExpiryDate = DateTime.Today.AddDays(3) },
            new Product { Name = "Whole Milk 2L", Brand = "FarmFresh", Barcode = "1000005", Price = 1.40m, Quantity = 30, Category = dairy, Supplier = metro, ExpiryDate = DateTime.Today.AddDays(10) },
            new Product { Name = "Cheddar 200g", Brand = "FarmFresh", Barcode = "1000006", Price = 2.50m, Quantity = 15, Category = dairy, Supplier = metro, ExpiryDate = DateTime.Today.AddMonths(2) },
            new Product { Name = "Bananas 1kg", Brand = "FreshPick", Barcode = "1000007", Price = 1.10m, Quantity = 4, Category = produce, Supplier = metro, ExpiryDate = DateTime.Today.AddDays(7) },
            new Product { Name = "Apples 1kg", Brand = "FreshPick", Barcode = "1000008", Price = 1.60m, Quantity = 50, Category = produce, Supplier = metro, ExpiryDate = DateTime.Today.AddDays(14) }
        );
        db.SaveChanges();
    }
}
