using ACME.Backend.Models;
using ACME.Backend.Tools.Converters;
using ACME.DataLayer.Entities;
using ACME.DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Backend.ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase, IController<ProductModel>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IUnitOfWork uow, ILogger<ProductController> logger)
    {
        _repo = uow.Products;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<ProductModel>> GetAsync(int page = 1,  int count = 20)
    {
        var  result =  await _repo.GetAllAsync(page, count);
        return result.Select(p => p.ToModel());
    }
    [HttpGet("{id}")]
    public async Task<ProductModel> GetByIdAsync(long id)
    {
        var p = await _repo.GetByIdAsync(id);
        return p.ToModel();
    }
    [HttpGet("{id}/prices")]
    public async Task<IEnumerable<PriceModel>> GetPricesAsync(long id, int page=1, int count=20)
    {
        var prices = await _repo.GetPricesAsync(id, page, count);
        return prices.Select(p => p.ToModel());
    }
    [HttpGet("{id}/expertreviews")]
    public async Task<IEnumerable<ReviewModel>> GetExpertReviewsAsync(long id, int page = 1, int count = 20)
    {
        var reviews = await _repo.GetReviewsAsync<ExpertReview>(id, page, count);
        return reviews.Select(p => p.ToModel());
    }
    [HttpGet("{id}/consumerreviews")]
    public async Task<IEnumerable<ReviewModel>> GetConsumerReviewsAsync(long id, int page = 1, int count = 20)
    {
        var reviews = await _repo.GetReviewsAsync<ConsumerReview>(id, page, count);
        return reviews.Select(p => p.ToModel());
    }
    [HttpGet("{id}/webreviews")]
    public async Task<IEnumerable<ReviewModel>> GetWebReviewsAsync(long id, int page = 1, int count = 20)
    {
        var reviews = await _repo.GetReviewsAsync<WebReview>(id, page, count);
        return reviews.Select(p => p.ToModel());
    }
    [HttpGet("{id}/specifications")]
    public async Task<IEnumerable<SpecificationModel>> GetSpecificationsAsync(long id, int page = 1, int count = 20)
    {
        var specs = await _repo.GetSpecificationsAsync(id, page, count);
        return specs.Select(p => p.ToModel());
    }
    [HttpPost]
    public async Task<ActionResult> InsertAsync(ProductModel model)
    {
        if (!ModelState.IsValid)
        {
            return Problem(string.Join(";\r\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }
        var entity = model.ToEntity();
        try
        {
            await _repo.InsertAsync(entity);
            var nr = await _repo.SaveAsync();
            if (nr > 0)
            {
                return CreatedAtAction("GetById", new { id = entity.Id }, null);
            }
            return Problem("Could not save this entity");
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(long Id, ProductModel model)
    {
        model.Id = Id;
        if (!ModelState.IsValid)
        {
            return Problem(string.Join(";\r\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }
        var entity = model.ToEntity();
        try
        {
            await _repo.UpdateAsync(Id, entity);
            var nr = await _repo.SaveAsync();
            if (nr > 0)
            {
                return Ok();
            }
            return Problem("Could not save this entity");
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(long Id)
    {
        try
        {
            await _repo.DeleteAsync(Id);
            var nr = await _repo.SaveAsync();
            if (nr > 0)
            {
                return Ok();
            }
            return Problem("Could not save this entity");
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}