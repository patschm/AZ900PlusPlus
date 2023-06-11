using System.ComponentModel.DataAnnotations;

namespace ACME.Backend.Models;

public class ProductModel: Model
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public long BrandId { get; set; }
    public string? BrandName { get; set; }
    [Required]
    public long ProductGroupId { get; set; }
    public string? ProductGroupName { get; set; }
    public string? Image { get; set; }
}
