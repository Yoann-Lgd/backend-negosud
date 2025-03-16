using backend_negosud.Entities;
using backend_negosud.Repository.Interfaces;

namespace backend_negosud.Repository;

public class ReglementRepository : RepositoryBase<Reglement>, IReglementRepository
{
    public ReglementRepository(PostgresContext context) : base(context)
    {
            
    }
}