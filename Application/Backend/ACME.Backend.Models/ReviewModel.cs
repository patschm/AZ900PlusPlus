namespace ACME.Backend.Models;

public class ReviewModel: Model
{
    public string? Text { get; set; }
    public byte Score { get; set; }
    public long ProductId { get; set; }
    public long ReviewerId { get; set; }
    public ReviewType ReviewType { get; set; }
    public DateTime DateBought { get; set; }
    public string? Organization { get; set; }
    public string? ReviewUrl { get; set; }
}
