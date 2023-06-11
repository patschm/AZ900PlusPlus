using ACME.Backend.Models;
using ACME.DataLayer.Entities;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ACME.Backend.Tools.Converters;

public static class EntityExtensions
{
    public static BrandModel ToModel(this Brand brand)
    {
        return new BrandModel
        {
            Id = brand.Id,
            Name = brand.Name,
            Website = brand.Website
        };
    }
    public static PriceModel ToModel(this Price price)
    {
        return new PriceModel
        {
            Id = price.Id,
            BasePrice = price.BasePrice,
            PriceDate = price.PriceDate,
            ProductId = price.ProductId,
            ShopName = price.ShopName
        };
    }
    public static ProductGroupModel ToModel(this ProductGroup group)
    {
        return new ProductGroupModel
        {
            Id = group.Id,
            Name = group.Name,
            Image = group.Image
        };
    }
    public static ProductModel ToModel(this Product product)
    {
        return new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Image = product.Image,
            BrandId = product.Brand.Id,
            BrandName = product.Brand?.Name,
            ProductGroupId = product.ProductGroup.Id,
            ProductGroupName = product.ProductGroup?.Name
        };
    }
    public static ReviewerModel ToModel(this Reviewer reviewer)
    {
        return new ReviewerModel
        {
            Id = reviewer.Id,
            Name = reviewer.Name,
            Email = reviewer.Email,
            UserName = reviewer.UserName
        };
    }
    public static ReviewModel ToModel(this Review review)
    {
        var model = new ReviewModel { 
            Id = review.Id,
            ProductId = review.ProductId,
            Score = review.Score,
            Text = review.Text,
            ReviewerId = review.ReviewerId
        };
        switch(review.ReviewType)
        {
            case DataLayer.Entities.ReviewType.Expert:
                model.ReviewType = Models.ReviewType.Expert;
                if (review is ExpertReview expert)
                    model.Organization = expert.Organization;
                break;
            case DataLayer.Entities.ReviewType.Consumer:
                model.ReviewType = Models.ReviewType.Consumer;
                if (review is ConsumerReview consumer)
                    model.DateBought = consumer.DateBought;
                break;
            case DataLayer.Entities.ReviewType.Web:
                model.ReviewType = Models.ReviewType.Web;
                if (review is WebReview web)
                    model.ReviewUrl = web.ReviewUrl;
                break;
            default:
                model.ReviewType = Models.ReviewType.Generic;
                break;
        }
        return model;
    }
    public static SpecificationDefinitionModel ToModel(this SpecificationDefinition specdef)
    {
        return new SpecificationDefinitionModel
        {
            Id = specdef.Id,
            Name = specdef.Name,
            Description = specdef.Description,
            Key = specdef.Key,
            ProductGroupId = specdef.ProductGroupId,
            Type = specdef.Type,
            Unit = specdef.Unit
        };
    }
    public static SpecificationModel ToModel(this Specification spec)
    {
        var specification = new SpecificationModel
        {
            Id = spec.Id,
            Key = spec.Key,
            ProductId = spec.ProductId
        };
        if (spec.BoolValue != null)
        {
            specification.Value = spec.BoolValue;
        }
        if (spec.NumberValue != null)
        {
            specification.Value = spec.NumberValue;
        }
        if (spec.StringValue != null)
        {
            if (spec.StringValue.StartsWith("[") && spec.StringValue.EndsWith("]"))
                specification.Value = JsonConvert.DeserializeObject<string[]>(spec.StringValue);
            else
                specification.Value = spec.StringValue;
        }
        return specification;
    }
}
