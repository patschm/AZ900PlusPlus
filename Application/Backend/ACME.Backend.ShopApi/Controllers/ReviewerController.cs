using ACME.Backend.Models;
using ACME.Backend.Tools.Converters;
using ACME.DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Backend.ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewerController : ControllerBase, IController<ReviewerModel>
{
    private readonly IReviewerRepository _repo;
    private readonly ILogger<ReviewerController> _logger;

    public ReviewerController(IUnitOfWork uow, ILogger<ReviewerController> logger)
    {
        _repo = uow.Reviewers;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<ReviewerModel>> GetAsync(int page = 1,  int count = 20)
    {
        var  result =  await _repo.GetAllAsync(page, count);
        return result.Select(p=>p.ToModel());
    }
    [HttpGet("{id}")]
    public async Task<ReviewerModel> GetByIdAsync(long id)
    {
        var p = await _repo.GetByIdAsync(id);
        return p.ToModel();
    }
    [HttpGet("{id}/reviews")]
    public async Task<IEnumerable<ReviewModel>> GetReviewsAsync(long id, int page = 1, int count = 20)
    {
        var reviews = await _repo.GetExpertReviewsAsync(id, page, count);
        return reviews.Select(p => p.ToModel());
    }
    [HttpGet("{id}/expertreviews")]
    public async Task<IEnumerable<ReviewModel>> GetExpertReviewsAsync(long id, int page = 1, int count = 20)
    {
        var reviews = await _repo.GetExpertReviewsAsync(id, page, count);
        return reviews.Select(p => p.ToModel());
    }
    [HttpGet("{id}/consumerreviews")]
    public async Task<IEnumerable<ReviewModel>> GetConsumerReviewsAsync(long id, int page = 1, int count = 20)
    {
        var reviews = await _repo.GetConsumerReviewsAsync(id, page, count);
        return reviews.Select(p => p.ToModel());
    }
    [HttpGet("{id}/webreviews")]
    public async Task<IEnumerable<ReviewModel>> GetWebReviewsAsync(long id, int page = 1, int count = 20)
    {
        var reviews = await _repo.GetWebReviewsAsync(id, page, count);
        return reviews.Select(p => p.ToModel());
    }
    [HttpPost]
    public async Task<ActionResult> InsertAsync(ReviewerModel model)
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
    public async Task<ActionResult> UpdateAsync(long Id, ReviewerModel model)
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