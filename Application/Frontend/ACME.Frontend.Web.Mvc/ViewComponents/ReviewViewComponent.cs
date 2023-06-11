using ACME.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Frontend.Web.Mvc.ViewComponents;

public class ReviewViewComponent: ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReviewViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(long pid)
    {
        var client = _httpClientFactory.CreateClient("data");
        var tasks = new Task<ReviewModel[]>[3];
        tasks[0]= client.GetFromJsonAsync<ReviewModel[]>($"product/{pid}/expertreviews?page=1&count=200")!;
        tasks[1] = client.GetFromJsonAsync<ReviewModel[]>($"product/{pid}/consumerreviews?page=1&count=200")!;
        tasks[2] = client.GetFromJsonAsync<ReviewModel[]>($"product/{pid}/webreviews?page=1&count=200")!;
        await Task.WhenAll( tasks );
        var data = tasks.SelectMany(t => t.Result).ToArray();
        return View(data);
    }
}
