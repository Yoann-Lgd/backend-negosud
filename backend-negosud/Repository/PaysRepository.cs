using backend_negosud.Entities;
using backend_negosud.Repository.Interfaces;

namespace backend_negosud.Repository;

public class PaysRepository : RepositoryBase<Pays>, IPaysRepository
{
    public PaysRepository(PostgresContext context) : base(context)
    {
        
    }
}