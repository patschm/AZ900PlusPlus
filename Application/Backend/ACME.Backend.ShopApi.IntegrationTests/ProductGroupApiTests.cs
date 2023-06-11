using ACME.Backend.Models;
using ACME.DataLayer.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ACME.Backend.ShopApi.IntegrationTests;

public class ProductGroupApiTests: BaseApiTest<ProductGroup>
{
    
    public ProductGroupApiTests(TestWebApplicationFactory<Program> factory): base(factory,ApiPaths.PRODUCTGROUP)
    {
    }

    [Theory]
    [InlineData(1, 3)]
    public override async Task Test_GetPagedAsync(int page, int count)
    {
        await base.Test_GetPagedAsync(page, count);
    }

    [Theory]
    [InlineData(3)]
    public override async Task Test_GetAsync(uint id)
    {
        await base.Test_GetAsync(id);
    }
    [Theory]
    [InlineData(4, 1, 2)]
    public async Task Test_GetProductsAsync(uint id, int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/{id}/products?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Product[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<Product[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    public async Task Test_GetSpecDefinitionsAsync(uint id)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/{id}/specdefinitions");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Product[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<Product[]>(data);
        Assert.True(data?.Length > 0);
    }
    [Fact]
    public async Task Test_PostAsync()
    {
        var model = new ProductGroupModel {Id=1000_000_000, Name = "Test", Image="group.jpg" };
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"{ApiPath}", 
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
        var location = response.Headers.Location;

        Assert.True(response.StatusCode == HttpStatusCode.Created);
        Assert.NotNull(location);   
    }
    [Theory]
    [InlineData(2)]
    public async Task Test_PutAsync(uint id)
    {
        var model = new ProductGroupModel { Id = id, Name = "Test" };
        var client = _factory.CreateClient();
        
        var response = await client.PutAsync($"{ApiPath}/{id}",
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
        
        Assert.True(response.StatusCode == HttpStatusCode.OK);
    }
    [Theory]
    [InlineData(1)]
    public override async Task Test_DeleteAsync(uint id)
    {
        await base.Test_DeleteAsync(id);
    }
}