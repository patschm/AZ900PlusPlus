using ACME.DataLayer.Entities;
using ACME.DataLayer.Interfaces;

namespace ACME.DataLayer.Stubs;

public class PriceRepository : BaseRepository<Price>, IPriceRepository
{
    public PriceRepository(InMemoryContext context) : base(context)
    {
    }
}
