using ACME.Backend.Models;
using ACME.DataLayer.Entities;
using Newtonsoft.Json;

namespace ACME.Backend.Tools.Converters;

public static class ModelExtensions
{
    public static Brand ToEntity(this BrandModel model)
    {
        return new Brand
        {
            Id = model.Id,
            Name = model.Name,
            Website = model.Website
        };
    }
    public static Price ToEntity(this PriceModel model)
    {
        return new Price
        {
            Id = model.Id,
            BasePrice = model.BasePrice,
            PriceDate = model.PriceDate,
            ProductId = model.ProductId,
            ShopName = model.ShopName
        };
    }
    public static ProductGroup ToEntity(this ProductGroupModel model)
    {
        return new ProductGroup
        {
            Id = model.Id,
            Name = model.Name,
            Image = model.Image
        };
    }
    public static Product ToEntity(this ProductModel model)
    {
        return new Product
        {
            Id = model.Id,
            Name = model.Name,
            Image = model.Image,
            BrandId = model.BrandId,
            ProductGroupId = model.ProductGroupId
        };
    }
    public static Reviewer ToEntity(this ReviewerModel model)
    {
        return new Reviewer
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email,
            UserName = model.UserName
        };
    }
    public static Review ToEntity(this ReviewModel model)
    {
        Review review;
        switch (model.ReviewType)
        {
            case Models.ReviewType.Expert:
                review = new ExpertReview { Organization = model.Organization };
                break;
            case Models.ReviewType.Consumer:
                review = new ConsumerReview { DateBought = model.DateBought };
                break;
            case Models.ReviewType.Web:
                review = new WebReview { ReviewUrl = model.ReviewUrl };
                break;
            default:
                review = new Review { ReviewType = DataLayer.Entities.ReviewType.Generic };
                break;
        }
        review.Id = model.Id;
        review.ProductId = model.ProductId;
        review.Score = model.Score;
        review.Text = model.Text;
        review.ReviewerId = model.ReviewerId;
        return review;
    }
    public static SpecificationDefinition ToEntity(this SpecificationDefinitionModel model)
    {
        return new SpecificationDefinition
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Key = model.Key,
            ProductGroupId = model.ProductGroupId,
            Type = model.Type,
            Unit = model.Unit
        };
    }
    public static Specification ToEntity(this SpecificationModel model)
    {
        var specification = new Specification
        {
            Id = model.Id,
            Key = model.Key,
            ProductId = model.ProductId
        };
        if (model.Value != null && model.Value is bool)
        {
            specification.BoolValue = (bool)model.Value;
        }
        else if (model.Value is string[])
        {
            specification.StringValue = JsonConvert.SerializeObject(model.Value);
        }
        else if (model.Value is string)
        {
            specification.StringValue = (string)model.Value;
        }
        else if (model.Value != null)
        {
            specification.NumberValue = (double)model.Value;
        }

        return specification;
    }
}
