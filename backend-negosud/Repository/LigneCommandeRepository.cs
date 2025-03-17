using backend_negosud.Entities;
using backend_negosud.Repository.Interfaces;

namespace backend_negosud.Repository;

public class LigneCommandeRepository : RepositoryBase<LigneCommande>, ILigneCommandeRepository
{
    public LigneCommandeRepository(PostgresContext context) : base(context)
    {
    }
}