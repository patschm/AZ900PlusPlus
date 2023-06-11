using System.ComponentModel.DataAnnotations;

namespace ACME.Backend.Models;

public class BrandModel : Model
{
    [Required]
    [MaxLength(255)]
    public string? Name { get; set; }
    [MaxLength(1024)]
    public string? Website { get; set; }
}
