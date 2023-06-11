using ACME.DataLayer.Entities;
using ACME.DataLayer.Interfaces;

namespace ACME.DataLayer.Stubs;

public class SpecificationDefinitionRepository : BaseRepository<SpecificationDefinition>, ISpecificationDefinitionRepository
{
    public SpecificationDefinitionRepository(InMemoryContext context) : base(context)
    {
    }
}
