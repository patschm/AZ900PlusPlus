using ACME.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Frontend.Web.Mvc.ViewComponents;

public class SpecificationViewComponent: ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SpecificationViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(long pid)
    {
        var client = _httpClientFactory.CreateClient("data");
        var data = await client.GetFromJsonAsync<SpecificationModel[]>($"product/{pid}/specifications?page=1&count=200");
        return View(data);
    }
}
