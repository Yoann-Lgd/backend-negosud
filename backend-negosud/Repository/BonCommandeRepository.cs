using backend_negosud.Entities;

namespace backend_negosud.Repository;

public class BonCommandeRepository : RepositoryBase<BonCommande>, IBonCommandeRepository
{
    public BonCommandeRepository(PostgresContext context) : base(context)
    {
    }
}