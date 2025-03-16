using backend_negosud.Entities;
using backend_negosud.Repository.Interfaces;

namespace backend_negosud.Repository;

public class FactureRepository : RepositoryBase<Facture>, IFactureRepository
{
    public FactureRepository(PostgresContext context) : base(context)
    {
            
    }
}