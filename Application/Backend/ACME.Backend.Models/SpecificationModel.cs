namespace ACME.Backend.Models;

public class SpecificationModel
{
    public long Id { get; set; }
    public string? Key { get; set; }
    public object? Value { get; set; }
    public long ProductId { get; set; }
}
