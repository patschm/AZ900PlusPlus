using ACME.Backend.Models;
using ACME.Backend.Tools.Converters;
using ACME.DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Backend.ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductGroupController : ControllerBase, IController<ProductGroupModel>
{
    private readonly IProductGroupRepository _repo;
    private readonly ILogger<ProductGroupController> _logger;

    public ProductGroupController(IUnitOfWork uow, ILogger<ProductGroupController> logger)
    {
        _repo = uow.ProductGroups;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<ProductGroupModel>> GetAsync(int page = 1,  int count = 20)
    {
        var result =  await _repo.GetAllAsync(page, count);
        return result.Select(p => p.ToModel());
    }
    [HttpGet("{id}")]
    public async Task<ProductGroupModel> GetByIdAsync(long id)
    {
        var result = await _repo.GetByIdAsync(id);
        return result.ToModel();
    }
    [HttpGet("{id}/products")]
    public async Task<IEnumerable<ProductModel>> GetProductsByGroupIdAsync(long id, int page = 1, int count = 10)
    {
        var result =  await _repo.GetProductsAsync(id, page, count);
        return result.Select(p => p.ToModel());
    }
    [HttpGet("{id}/specdefinitions")]
    public async Task<IEnumerable<SpecificationDefinitionModel>> GetSpecDefinitionsByGroupIdAsync(long id)
    {
        var result = await _repo.GetSpecificationDefinitionsAsync(id);
        return result.Select(p => p.ToModel());
    }
    [HttpPost]
    public async Task<ActionResult> InsertAsync(ProductGroupModel model)
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
    public async Task<ActionResult> UpdateAsync(long Id, ProductGroupModel model)
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