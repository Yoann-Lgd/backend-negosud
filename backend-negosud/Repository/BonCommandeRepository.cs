using backend_negosud.Entities;

namespace backend_negosud.Repository;

public class BonCommandeRepository : RepositoryBase<BonCommande>, IBonCommandeRepository
{
    PostgresContext _context;
    ILogger<BonCommandeRepository> _logger;
    public BonCommandeRepository(PostgresContext context, ILogger<BonCommandeRepository> logger) : base(context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    
}