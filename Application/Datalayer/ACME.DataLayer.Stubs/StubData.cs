using ACME.DataLayer.Entities;

namespace ACME.DataLayer.Stubs;

public class StubData
{
    public ICollection<Brand> Brands = new List<Brand>();
    public ICollection<Price> Prices = new List<Price>();
    public ICollection<Product> Products = new List<Product>();
    public ICollection<ProductGroup> ProductGroups = new List<ProductGroup>();
    public ICollection<Review> Reviews = new List<Review>();
    public ICollection<Reviewer> Reviewers = new List<Reviewer>();
    public ICollection<Specification> Specifications = new List<Specification>();
    public ICollection<SpecificationDefinition> SpecificationDefinitions = new List<SpecificationDefinition>();
    public ICollection<ConsumerReview> ConsumerReviews = new List<ConsumerReview>();
    public ICollection<ExpertReview> ExpertReviews = new List<ExpertReview>();
    public ICollection<WebReview> WebReviews = new List<WebReview>();

    public void Seed(int amount = 10)
    {
        var rnd = new Random();
        for (int i = 1; i <= amount; i++)
        {
            var brand = new Brand
            {
                Id = i,
                Name = $"Brand {i}",
                Website = $"https://brand_{i}"
            };
            Brands.Add(brand);
            var group = new ProductGroup
            {
                Id = i,
                Name = $"ProductGroup {i}",
                Image = $"productgroup_image_{i}.jpg"
            };
            ProductGroups.Add(group);
            var reviewer = new Reviewer
            {
                Id = i,
                Email = $"tester{i}@test.nl",
                Name = $"Tester {i}",
                UserName = $"tester{i}",
            };
            Reviewers.Add(reviewer);
            var specdef = new SpecificationDefinition
            {
                Id = i,
                Description = $"Description {i}",
                Key = $"key_{i}",
                Name = $"Name {i}",
                ProductGroupId = group.Id,
                Type = $"type{rnd.Next(1, 4)}",
                Unit = $"Unit{i}"
            };
            SpecificationDefinitions.Add(specdef);
            long max = (i - 1) * amount + amount;
            for (long j = (i - 1) * amount + 1; j <= max; j++)
            {
                var product = new Product
                {
                    Id = j,
                    Name = $"Product {j}",
                    BrandId = brand.Id,
                    ProductGroupId = group.Id,
                    Image = $"product_{j}.jpg"
                };
                Products.Add(product);
                var creview = new ConsumerReview
                {
                    Id = j,
                    DateBought = DateTime.Now.AddMonths(-(int)j),
                    ReviewerId = reviewer.Id,
                    Score = (byte)(j % 5),
                    Text = $"Text {j}",
                    ProductId = product.Id  
                };
                ConsumerReviews.Add(creview);
                var ereview = new ExpertReview
                {
                    Id = max * max + j,
                    Organization = $"Company {max * max + j}",
                    ReviewerId = reviewer.Id,
                    Score = (byte)(j % 5),
                    Text = $"Text {max * max + j}",
                    ProductId = product.Id
                };
                ExpertReviews.Add(ereview);
                var wreview = new WebReview
                {
                    Id = 2 * max * max + j,
                    ReviewUrl = $"https://test{2 * max * max + j}.nl/review{2 * max * max + j}",
                    ReviewerId = reviewer.Id,
                    Score = (byte)(j % 5),
                    Text = $"Text {2 * max * max + j}",
                    ProductId = product.Id
                };
                WebReviews.Add(wreview);
                var price = new Price
                {
                    Id = j,
                    BasePrice = rnd.Next(10, 1000),
                    ShopName = $"Shop {j}",
                    ProductId = product.Id,
                    PriceDate = DateTime.Now.AddDays(-j)
                };
                Prices.Add(price);
                var spec = new Specification
                {
                    Id = j,
                    Key = $"key_{j}",
                    ProductId = product.Id,
                    BoolValue = specdef.Type == "type1" ? rnd.Next(0, 2) == 1 : null,
                    NumberValue = specdef.Type == "type2" ? j : null,
                    StringValue = specdef.Type == "type3" ? $"value {j}" : null
                };
                Specifications.Add(spec);
            }
        }
    }
}