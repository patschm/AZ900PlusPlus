using ACME.Backend.Models;
using ACME.DataLayer.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ACME.Backend.ShopApi.IntegrationTests;

public class ProductApiTests: BaseApiTest<Product>
{
    public ProductApiTests(TestWebApplicationFactory<Program> factory): base(factory, ApiPaths.PRODUCT)
    {
    
    }

    [Theory]
    [InlineData(1, 12)]
    public override async Task Test_GetPagedAsync(int page, int count)
    {
        await base.Test_GetPagedAsync(page, count);
    }

    [Theory]
    [InlineData(5)]
    public override async Task Test_GetAsync(uint id)
    {
        await base.Test_GetAsync(id);
    }
    [Theory]
    [InlineData(5, 1, 1)]
    public async Task Test_GetWebReviewsAsync(uint id, int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/{id}/webreviews?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ReviewModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<ReviewModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(5, 1, 1)]
    public async Task Test_GetExpertReviewsAsync(uint id, int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/{id}/expertreviews?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ReviewModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<ReviewModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(5, 1, 1)]
    public async Task Test_GetConsumerReviewsAsync(uint id, int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/{id}/consumerreviews?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ReviewModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<ReviewModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(1, 1, 1)]
    public async Task Test_GetPricesAsync(uint id, int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/{id}/prices?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<PriceModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<PriceModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(5, 1, 1)]
    public async Task Test_GetSpecificationsAsync(uint id, int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/{id}/specifications?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<SpecificationModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<SpecificationModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Fact]
    public async Task Test_PostAsync()
    {
        var model = new ProductModel { Name = "Test", Id=1_000_000_000_000, BrandId=1, ProductGroupId=1 };
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
        var model = new ProductModel { Id = id, ProductGroupId=1, BrandId=1, Name = "Test" };
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