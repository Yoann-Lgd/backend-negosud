using AutoMapper;
using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

    public async Task<Commande> GetActiveBasketByClientIdAsync(int clientId)
    {
        return await _context.Commandes
            .Where(c => c.ClientId == clientId && !c.Valide)
            .Include(c => c.LigneCommandes)
            .ThenInclude(l => l.Article)
            .ThenInclude(a => a.Famille) // Inclure la relation Famille ici
            .OrderByDescending(c => c.DateCreation)
            .FirstOrDefaultAsync();
    }
    
    public async Task UpdateCommandeFieldsAsync(Commande commande)
    {
        try
        {
            var existingEntity = await _context.Commandes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CommandeId == commande.CommandeId);

            if (existingEntity == null)
            {
                throw new Exception($"Commande avec ID {commande.CommandeId} non trouvée.");
            }
            
            var sql = @"
            UPDATE commande 
            SET valide = @Valide, facture_id = @FactureId, status = @Status
            WHERE commande_id = @CommandeId";
            
            await _context.Database.ExecuteSqlRawAsync(sql,
                new Npgsql.NpgsqlParameter("@Valide", commande.Valide),
                new Npgsql.NpgsqlParameter("@FactureId", commande.FactureId),
                new Npgsql.NpgsqlParameter("@Status", commande.Status),
                new Npgsql.NpgsqlParameter("@CommandeId", commande.CommandeId));
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur lors de la mise à jour des champs de la commande : {ex.Message}", ex);
        }
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
    
    
}