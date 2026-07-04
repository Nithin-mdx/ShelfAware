using Microsoft.AspNetCore.Mvc.RazorPages;
using Supermarket.App.Models;
using Supermarket.App.Services;

namespace Supermarket.App.Pages;

public class SalesModel : PageModel
{
    private readonly SalesServ _sales;
    public SalesModel(SalesServ sales) => _sales = sales;

    public List<Sale> Sales = new();

    public void OnGet() => Sales = _sales.History();
}
