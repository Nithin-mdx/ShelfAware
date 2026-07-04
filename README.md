# ShelfAware — Local Supermarket Management System

An **ASP.NET Core (Razor Pages)** web supermarket management system for small shops, built
with **C# / .NET 8**, **Entity Framework Core**, and **SQL Server**. It manages products,
categories, suppliers, stock, sales and reports, and uses **custom data structures and search
algorithms** for efficient in-memory searching. The UI runs in the browser at
`http://localhost:5080`.

> Coursework: CST2550 — *Local Supermarket Management System for Small Shops*.

## Features

- **Products** — add, update, delete, list (with validation).
- **Categories & Suppliers** — full CRUD.
- **Stock** — restock, adjust, low-stock alerts; every change logged in a stock-movement table.
- **Sales** — multi-item sale, automatic stock reduction, receipt, saved history.
- **Search** — four algorithms: BST (name), hash table (barcode), linear (contains / category / supplier), quicksort + binary search (name).
- **Reports** — low stock, sales by product, products by category, supplier stock.
- **Dashboard** — totals for products, suppliers, sales and low-stock items.

## Project structure

```
src/Supermarket.Core           class library (all logic, reused by web app + tests)
  Models/            EF entities (Product, Category, Supplier, Sale, SaleItem, StockMovement)
  DataStructures/    CstmLinkedList, BST, Hashtable  (custom, no built-in collections)
  Algorithms/        LinearSearch, BinarySearch, QuickSort  (with complexity notes)
  Data/              SupermrktCont (EF Core), Seed
  Services/          ProductServ, CatalogServ, SalesServ, ReportServ
src/Supermarket.App            ASP.NET Core Razor Pages web app
  Pages/             Dashboard, Products, Catalog, Stock, Search, Sell, Reports
  Program.cs         web host + dependency injection
tests/Supermarket.Tests        xUnit tests (EF Core InMemory)
Database/CreateDatabase.sql    SQL Server creation script
report/Report.md               coursework report
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- **SQL Server** — any SQL Server instance (Express, Developer, or LocalDB). The default
  connection string targets a local SQL Server Express instance and is in
  `src/Supermarket.Core/Data/SupermrktCont.cs`:

  ```
  Server=.\SQLEXPRESS;Database=SupermarketDb;Trusted_Connection=True;TrustServerCertificate=True
  ```

  Edit that string to point at any SQL Server instance you prefer.

## Database setup

Either let the app create the database automatically (it calls `EnsureCreated`
on first launch and seeds sample data), **or** run the script manually:

```bash
sqlcmd -S ".\SQLEXPRESS" -i Database/CreateDatabase.sql
```

## Build and run

```bash
dotnet build
dotnet run --project src/Supermarket.App
```

Then open **http://localhost:5080** in your browser. On first run the database is created and
seeded with sample categories, suppliers and products, so every page has data immediately.

## Run the tests

```bash
dotnet test
```

The tests use the EF Core **InMemory** provider, so they run without a SQL Server instance.
They cover: adding products, duplicate-barcode validation, positive-price validation, stock
updates, barcode/name search, sale recording (and stock reduction), insufficient-stock
rejection, low-stock reporting, and all three custom data structures plus the algorithms.

## Time complexity (summary)

| Operation                 | Structure / algorithm        | Complexity (avg) |
|---------------------------|------------------------------|------------------|
| Add product               | Linked list append + EF      | O(1) + DB        |
| Search by name (exact)    | Binary search tree           | O(log n)         |
| Search by name (contains) | Linear search                | O(n)             |
| Search by name (sorted)   | QuickSort + binary search    | O(n log n) / O(log n) |
| Barcode lookup            | Hash table                   | O(1)             |
| Stock update              | Lookup + update              | O(1) + DB        |
| Low-stock report          | Scan products                | O(n)             |


