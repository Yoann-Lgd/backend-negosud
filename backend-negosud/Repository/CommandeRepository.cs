using AutoMapper;
using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class CommandeRepository : RepositoryBase<Commande> , ICommandeRepository
{
    private readonly IMapper _mapper;

    private readonly ILogger<CommandeRepository> _logger;
    public CommandeRepository(
        PostgresContext context, 
        IMapper mapper, 
        ILogger<CommandeRepository> logger):base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<List<Commande>> GetAllCommandeAsync()
    {
        return await _context.Commandes
            .Include(c => c.LigneCommandes)
            .ThenInclude(lc => lc.Article) // si `LigneCommande` a un `Produit`
            .ToListAsync();
    }

    public async Task<Commande> GetByIdAndLigneCommandesAsync(int id)
    {
        return await _context.Commandes.Include(c => c.LigneCommandes).ThenInclude(l => l.Article).FirstOrDefaultAsync(c => c.CommandeId == id);
    }

}