using Microsoft.EntityFrameworkCore;
using Supermarket.App.Data;
using Supermarket.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<SupermrktCont>(o => o.UseSqlServer(SupermrktCont.DefaultConnection));
builder.Services.AddScoped<ProductServ>();
builder.Services.AddScoped<CatalogServ>();
builder.Services.AddScoped<SalesServ>();
builder.Services.AddScoped<ReportServ>();

var app = builder.Build();

// Create the database and seed sample data on first run.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupermrktCont>();
    db.Database.EnsureCreated();
    Seed.EnsureSeeded(db);
}

app.UseStaticFiles();
app.MapRazorPages();
app.Run();
