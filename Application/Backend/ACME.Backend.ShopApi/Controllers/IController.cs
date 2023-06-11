using ACME.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Backend.ShopApi.Controllers;

public interface IController<T>
{
    Task<IEnumerable<T>> GetAsync(int page = 1, int count = 20);
    Task<T> GetByIdAsync(long id);
    Task<ActionResult> InsertAsync(T model);
    Task<ActionResult> UpdateAsync(long Id, T model);
    Task<ActionResult> DeleteAsync(long Id);
}
