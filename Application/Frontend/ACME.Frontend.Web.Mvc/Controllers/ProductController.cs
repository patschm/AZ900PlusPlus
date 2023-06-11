using ACME.Backend.Models;
using ACME.Frontend.Web.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ACME.Frontend.Web.Mvc.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly IHttpClientFactory _httpFactory;

    public ProductController(ILogger<ProductController> logger, IHttpClientFactory httpFactory)
    {
        _logger = logger;
        _httpFactory = httpFactory;
    }

    public async Task<IActionResult> ProductgroupsAsync()
    {
        var http = _httpFactory.CreateClient("data");
        var model = await http.GetFromJsonAsync<ProductGroupModel[]>("productGroup?page=1&count=20");
        return View(model);
    }
    public async Task<IActionResult> AllProductsAsync(int page = 1, int count = 20)
    {
        var http = _httpFactory.CreateClient("data");
        var model = await http.GetFromJsonAsync<ProductModel[]>($"product?page={page}&count={count}");
        ViewBag.Page = page;
        ViewBag.HasNext = model?.Count() == count;
        return View(model);
    }
    public async Task<IActionResult> ProductsAsync(long pgid, int page = 1, int count = 20)
    {
        var http = _httpFactory.CreateClient("data");
        var model = await http.GetFromJsonAsync<ProductModel[]>($"productgroup/{pgid}/products?page={page}&count={count}");
        ViewBag.GroupId = pgid;
        ViewBag.Page = page;
        ViewBag.HasNext = model?.Count() == count;
        return View(model);
    }
    public async Task<IActionResult> ProductDetailAsync(long pid)
    {
        var http = _httpFactory.CreateClient("data");
        var model = await http.GetFromJsonAsync<ProductModel>($"product/{pid}");
        return View(model);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}