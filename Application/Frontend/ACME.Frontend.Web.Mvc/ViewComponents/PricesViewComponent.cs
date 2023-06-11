using ACME.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Frontend.Web.Mvc.ViewComponents;

public class PricesViewComponent: ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PricesViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(long pid)
    {
        var client = _httpClientFactory.CreateClient("data");
        var data = await client.GetFromJsonAsync<PriceModel[]>($"product/{pid}/prices?page=1&count=200");
        return View(data);
    }
}
