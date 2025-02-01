using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Enums;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository
{
    public class CommandeRepository : ICommandeRepository
    {
        private readonly PostgresContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CommandeRepository> _logger;

        public CommandeRepository(PostgresContext context, IMapper mapper, ILogger<CommandeRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<Commande> AddAsync(Commande entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Commandes.AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return entity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de l'ajout de la commande");
                throw new Exception("Une erreur s'est produite lors de l'ajout de la commande.", e);
            }
        }

        public Task<ResponseDataModel<CommandeOutputDto>> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            throw new NotImplementedException();
        }

        Task<ResponseDataModel<List<CommandeOutputDto>>> IRepositoryBase<Commande, CommandeOutputDto>.GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public async Task<Commande?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Commandes
                .Include(c => c.LigneCommandes)
                .ThenInclude(lc => lc.Article)
                .FirstOrDefaultAsync(c => c.CommandeId == id, cancellationToken);
        }
        
        Task<ResponseDataModel<CommandeOutputDto>> IRepositoryBase<Commande, CommandeOutputDto>.AddAsync(Commande entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Commande>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Commandes
                .Include(c => c.LigneCommandes)
                .ThenInclude(lc => lc.Article)
                .ToListAsync(cancellationToken);
        }

        // mettre à jour une commande
        public async Task UpdateAsync(Commande entity, CancellationToken cancellationToken = default)
        {
            _context.Commandes.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // supprimer une commande
        public async Task DeleteAsync(Commande entity, CancellationToken cancellationToken = default)
        {
            _context.Commandes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // récupérer une commande en fonction de son statut (Panier, Validée, etc.)
        public async Task<Commande?> GetCommandeByStatutAsync(int clientId, StatutCommande statut, CancellationToken cancellationToken = default)
        {
            return await _context.Commandes
                .Include(c => c.LigneCommandes)
                .ThenInclude(lc => lc.Article)
                .FirstOrDefaultAsync(c => c.ClientId == clientId && c.Statut == statut, cancellationToken);
        }
        
        public async Task<double> GetPrixProduitAsync(int articleId, CancellationToken cancellationToken = default)
        {
            var article = await _context.Articles.FindAsync(new object[] { articleId }, cancellationToken);
            
            if (article == null)
            {
                throw new Exception($"L'article avec l'ID {articleId} n'existe pas.");
            }
            
            return article.Prix;
        }

        public Task<Commande?> GetCommandeByStatut(int clientId, StatutCommande statut)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Commande commande)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Commande commande)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetPrixProduit(int produitId)
        {
            throw new NotImplementedException();
        }
    }
}
