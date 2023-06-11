using ACME.DataLayer.Entities;
using ACME.DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ACME.DataLayer.Stubs;

public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    public ReviewRepository(InMemoryContext context): base(context)
    {

    }
    public async Task<IEnumerable<ConsumerReview>> GetConsumerReviewsAsync(int page = 1, int count = 10)
    {
        return await Context.ConsumerReviews.Skip((page - 1) * count).Take(count).ToListAsync();
    }

    public async Task<IEnumerable<ExpertReview>> GetExpertReviewsAsync(int page = 1, int count = 10)
    {
        return await Context.ExpertReviews.Skip((page - 1) * count).Take(count).ToListAsync();
    }

    public async Task<IEnumerable<WebReview>> GetWebReviewsAsync(int page = 1, int count = 10)
    {
        return await Context.WebReviews.Skip((page - 1) * count).Take(count).ToListAsync();
    }
}
