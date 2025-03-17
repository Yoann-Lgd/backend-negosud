using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class BonCommandeRepository : RepositoryBase<BonCommande>, IBonCommandeRepository
{
    PostgresContext _context;
    ILogger<BonCommandeRepository> _logger;
    public BonCommandeRepository(PostgresContext context, ILogger<BonCommandeRepository> logger) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<List<BonCommande>> GetAllCommandeAsync()
    {
        return await _context.BonCommandes
            .Include(b => b.Fournisseur)
            .Include(b => b.LigneBonCommandes)
            .ThenInclude(lc => lc.Article)
            .ToListAsync();
    }    
    
    public async Task<BonCommande> GetById(int id)
    {
        return await _context.BonCommandes
            .Include(b => b.Fournisseur)
            .Include(b => b.LigneBonCommandes)
            .ThenInclude(lc => lc.Article)
            .FirstOrDefaultAsync(b => b.BonCommandeId == id);
    }
}