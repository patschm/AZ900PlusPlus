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

public class ReviewerApiTests: BaseApiTest<Reviewer>
{
    public ReviewerApiTests(TestWebApplicationFactory<Program> factory): base(factory, ApiPaths.REVIEWER)
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
    [InlineData(5, 1, 4)]
    public async Task Test_GetExpertReviewsAsync(long id, int page, int count)
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
    [InlineData(5, 1,4)]
    public async Task Test_GetConsumerReviewsAsync(long id, int page, int count)
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
    [InlineData(5, 1, 4)]
    public async Task Test_GetWebReviewsAsync(long id, int page, int count)
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
    [Fact]
    public async Task Test_PostAsync()
    {
        var model = new ReviewerModel {Id=1000_000_000, Name="Jan", Email="mail@mail.nl", UserName="jan"};
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"{ApiPath}", 
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
        var location = response.Headers.Location;

        Assert.True(response.StatusCode == HttpStatusCode.Created);
        Assert.NotNull(location);   
    }
    [Theory]
    [InlineData(1)]
    public async Task Test_PutAsync(long id)
    {
        var model = new ReviewerModel { Id = id, Name="Jan", Email = "mail@mail.nl", UserName="jan" };
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