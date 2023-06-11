using ACME.DataLayer.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ACME.Backend.ShopApi.IntegrationTests;
public class BaseApiTest<T> : IClassFixture<TestWebApplicationFactory<Program>> where T : Entity
{
    protected readonly TestWebApplicationFactory<Program> _factory;
    private readonly string _startPath;
    protected string ApiPath => _startPath;

    public BaseApiTest(TestWebApplicationFactory<Program> factory, string startPath)
    {
        _startPath = startPath;
        _factory = factory;
    }
    public virtual async Task Test_GetPagedAsync(int page, int count)
    {
        var client = _factory.CreateClient();
        var respose = await client.GetAsync($"{_startPath}?page={page}&count={count}");
        var stringData = await respose.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<T[]>(stringData);

        Assert.True(respose.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.True(data?.Length == count);
    }
    public virtual async Task Test_GetAsync(uint id)
    {
        var client = _factory.CreateClient();
        var respose = await client.GetAsync($"{_startPath}/{id}");
        var stringData = await respose.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<T>(stringData);

        Assert.True(respose.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<T>(data);
        Assert.True(data?.Id == id);
    }
    public virtual async Task Test_DeleteAsync(uint id)
    {
        var client = _factory.CreateClient();
        var respose = await client.DeleteAsync($"{_startPath}/{id}");

        Assert.True(respose.StatusCode == HttpStatusCode.OK);
    }
}
