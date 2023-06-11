using ACME.DataLayer.Entities;
using ACME.DataLayer.Interfaces;

namespace ACME.DataLayer.Stubs;

public class SpecificationRepository : BaseRepository<Specification>, ISpecificationRepository
{
    public SpecificationRepository(InMemoryContext context) : base(context)
    {
    }
}
