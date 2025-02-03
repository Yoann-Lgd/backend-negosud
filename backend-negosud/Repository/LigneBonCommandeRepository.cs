using backend_negosud.Entities;

namespace backend_negosud.Repository;

public class LigneBonCommandeRepository : RepositoryBase<LigneBonCommande>, ILigneBonCommandeRepository
{
    public LigneBonCommandeRepository(PostgresContext context) : base(context)
    {
    }
}