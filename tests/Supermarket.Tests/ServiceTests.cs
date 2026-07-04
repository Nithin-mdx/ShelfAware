using Microsoft.EntityFrameworkCore;
using Supermarket.App.Data;
using Supermarket.App.Models;
using Supermarket.App.Services;
using Xunit;

namespace Supermarket.Tests;

public class ProductServTests
{
    private readonly SupermrktCont _db;
    private readonly ProductServ _service;
    private readonly int _catId;
    private readonly int _supId;

    public ProductServTests()
    {
        var options = new DbContextOptionsBuilder<SupermrktCont>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new SupermrktCont(options);

        var cat = new Category { Name = "Drinks" };
        var sup = new Supplier { Name = "Metro Cash & Carry" };
        _db.AddRange(cat, sup);
        _db.SaveChanges();
        _catId = cat.Id;
        _supId = sup.Id;

        _service = new ProductServ(_db);
    }

    private Product NewProduct(string barcode = "111") => new()
    {
        Name = "Cola", Barcode = barcode, Price = 1.5m, Quantity = 10,
        CategoryId = _catId, SupplierId = _supId
    };

    [Fact]
    public void Add_SavesProduct()
    {
        var p = _service.Add(NewProduct());
        Assert.True(p.Id > 0);
        Assert.Equal(1, _db.Products.Count());
    }

    [Fact]
    public void Add_DuplicateBarcode_Throws()
    {
        _service.Add(NewProduct("999"));
        Assert.Throws<ArgumentException>(() => _service.Add(NewProduct("999")));
    }

    [Fact]
    public void Add_ZeroPrice_Throws()
    {
        var p = NewProduct();
        p.Price = 0;
        Assert.Throws<ArgumentException>(() => _service.Add(p));
    }

    [Fact]
    public void Add_NegativePrice_Throws()
    {
        var p = NewProduct();
        p.Price = -2;
        Assert.Throws<ArgumentException>(() => _service.Add(p));
    }

    [Fact]
    public void Add_MissingName_Throws()
    {
        var p = NewProduct();
        p.Name = "";
        Assert.Throws<ArgumentException>(() => _service.Add(p));
    }

    [Fact]
    public void AdjustStock_ReducesQuantity()
    {
        var p = _service.Add(NewProduct());
        _service.AdjustStock(p.Id, -4, "Adjustment");
        Assert.Equal(6, _service.Get(p.Id)!.Quantity);
    }

    [Fact]
    public void AdjustStock_BelowZero_Throws()
    {
        var p = _service.Add(NewProduct());
        Assert.Throws<InvalidOperationException>(() => _service.AdjustStock(p.Id, -100, "Adjustment"));
    }

    [Fact]
    public void Delete_RemovesProduct()
    {
        var p = _service.Add(NewProduct());
        Assert.True(_service.Delete(p.Id));
        Assert.Equal(0, _db.Products.Count());
    }

    [Fact]
    public void Delete_MissingId_ReturnsFalse()
    {
        Assert.False(_service.Delete(999));
    }

    [Fact]
    public void SearchByBar_FindsProduct()
    {
        _service.Add(NewProduct("abc123"));
        var all = _service.LoadAll();
        var found = _service.SearchByBar(_service.BuildBarcodeIndex(all), "abc123");
        Assert.NotNull(found);
        Assert.Equal("Cola", found!.Name);
    }

    [Fact]
    public void SearchByBar_MissingBarcode_ReturnsNull()
    {
        _service.Add(NewProduct());
        var all = _service.LoadAll();
        Assert.Null(_service.SearchByBar(_service.BuildBarcodeIndex(all), "nope"));
    }

    [Fact]
    public void SearchByNameExact_FindsProduct()
    {
        _service.Add(NewProduct());
        var all = _service.LoadAll();
        var found = _service.SearchByNameExact(_service.BuildNameTree(all), "Cola");
        Assert.Equal(1, found.Count);
    }

    [Fact]
    public void BinarySearchByName_FindsProduct()
    {
        _service.Add(NewProduct());
        Assert.NotNull(_service.BinarySearchByName(_service.LoadAll(), "Cola"));
    }

    [Fact]
    public void SmartSearch_DigitsOnly_UsesHashTable()
    {
        _service.Add(NewProduct("111"));
        var result = _service.SmartSearch("111");
        Assert.Contains("hash", result.Method);
        Assert.Single(result.Results);
    }

    [Fact]
    public void SmartSearch_ExactName_UsesBst()
    {
        _service.Add(NewProduct());
        var result = _service.SmartSearch("Cola");
        Assert.Contains("tree", result.Method);
        Assert.Single(result.Results);
    }

    [Fact]
    public void SmartSearch_PartialText_UsesLinearScan()
    {
        _service.Add(NewProduct());
        var result = _service.SmartSearch("Drink");
        Assert.Contains("linear", result.Method);
        Assert.Single(result.Results);
    }
}

public class SalesServTests
{
    private readonly SupermrktCont _db;
    private readonly ProductServ _products;
    private readonly SalesServ _sales;
    private readonly Product _cola;

    public SalesServTests()
    {
        var options = new DbContextOptionsBuilder<SupermrktCont>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new SupermrktCont(options);

        var cat = new Category { Name = "Drinks" };
        var sup = new Supplier { Name = "Metro Cash & Carry" };
        _db.AddRange(cat, sup);
        _db.SaveChanges();

        _products = new ProductServ(_db);
        _sales = new SalesServ(_db);
        _cola = _products.Add(new Product
        {
            Name = "Cola", Barcode = "111", Price = 1.5m, Quantity = 10,
            CategoryId = cat.Id, SupplierId = sup.Id
        });
    }

    [Fact]
    public void Record_ReducesStock()
    {
        _sales.Record(new[] { (_cola.Id, 3) });
        Assert.Equal(7, _products.Get(_cola.Id)!.Quantity);
    }

    [Fact]
    public void Record_CalculatesTotal()
    {
        var sale = _sales.Record(new[] { (_cola.Id, 3) });
        Assert.Equal(4.5m, sale.Total);
    }

    [Fact]
    public void Record_SavesToHistory()
    {
        _sales.Record(new[] { (_cola.Id, 1) });
        Assert.Single(_sales.History());
    }

    [Fact]
    public void Record_NotEnoughStock_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => _sales.Record(new[] { (_cola.Id, 999) }));
    }

    [Fact]
    public void Record_NotEnoughStock_LeavesStockUnchanged()
    {
        try { _sales.Record(new[] { (_cola.Id, 999) }); } catch { }
        Assert.Equal(10, _products.Get(_cola.Id)!.Quantity);
    }

    [Fact]
    public void Record_ZeroQuantity_Throws()
    {
        Assert.Throws<ArgumentException>(() => _sales.Record(new[] { (_cola.Id, 0) }));
    }
}

public class ReportServTests
{
    private readonly SupermrktCont _db;
    private readonly ProductServ _products;
    private readonly ReportServ _reports;

    public ReportServTests()
    {
        var options = new DbContextOptionsBuilder<SupermrktCont>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new SupermrktCont(options);

        var cat = new Category { Name = "Drinks" };
        var sup = new Supplier { Name = "Metro Cash & Carry" };
        _db.AddRange(cat, sup);
        _db.SaveChanges();

        _products = new ProductServ(_db);
        _reports = new ReportServ(_db);
        _products.Add(new Product
        {
            Name = "Cola", Barcode = "111", Price = 1.5m, Quantity = 2,
            CategoryId = cat.Id, SupplierId = sup.Id
        });
    }

    [Fact]
    public void LowStock_IncludesProductBelowThreshold()
    {
        Assert.Single(_reports.LowStock(5));
    }

    [Fact]
    public void LowStock_ExcludesProductAboveThreshold()
    {
        Assert.Empty(_reports.LowStock(1));
    }

    [Fact]
    public void ProductsByCategory_CountsProducts()
    {
        var rows = _reports.ProductsByCategory();
        Assert.Single(rows);
        Assert.Equal("Drinks", rows[0].Category);
        Assert.Equal(1, rows[0].Products);
    }

    [Fact]
    public void SupplierStock_CountsUnits()
    {
        var rows = _reports.SupplierStock();
        Assert.Single(rows);
        Assert.Equal(2, rows[0].Units);
    }
}
