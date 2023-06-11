using ACME.Backend.Models;
using ACME.DataLayer.Entities;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ACME.Backend.ShopApi.IntegrationTests;

public class ReviewApiTests: BaseApiTest<Review>
{
    public ReviewApiTests(TestWebApplicationFactory<Program> factory): base(factory, ApiPaths.REVIEW)
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
    [InlineData(1, 4)]
    public async Task Test_GetExpertReviewsAsync(int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/expertreviews?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ReviewModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<ReviewModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(1,4)]
    public async Task Test_GetConsumerReviewsAsync(int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/consumerreviews?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ReviewModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<ReviewModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(1, 4)]
    public async Task Test_GetWebReviewsAsync(int page, int count)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"{ApiPath}/webreviews?page={page}&count={count}");
        var stringData = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ReviewModel[]>(stringData);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(data);
        Assert.IsType<ReviewModel[]>(data);
        Assert.True(data?.Length == count);
    }
    [Theory]
    [InlineData(Models.ReviewType.Consumer)]
    [InlineData(Models.ReviewType.Expert)]
    [InlineData(Models.ReviewType.Web)]
    public async Task Test_PostAsync(Models.ReviewType type)
    {
        var model = new ReviewModel {Id=1000_000_000, ReviewType=type, ProductId=1, ReviewerId=1};
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"{ApiPath}", 
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
        var location = response.Headers.Location;

        Assert.True(response.StatusCode == HttpStatusCode.Created);
        Assert.NotNull(location);   
    }
    [Theory]
    [InlineData(1, Models.ReviewType.Consumer)]
    [InlineData(11, Models.ReviewType.Expert)]
    [InlineData(21, Models.ReviewType.Web)]
    public async Task Test_PutAsync(uint id, Models.ReviewType type)
    {
        var model = new ReviewModel { Id = id, ReviewType=type, ProductId=1, ReviewerId=1 };
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