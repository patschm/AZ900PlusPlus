namespace ACME.Backend.Models;

public class SpecificationDefinitionModel: Model
{
    public string? Key { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public long ProductGroupId { get; set; }
}
