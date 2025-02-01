using backend_negosud.Entities;
using backend_negosud.Enums;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface ICommandeService
{
    Task<Commande> AddAsync(Commande commande, CancellationToken cancellationToken = default);
    Task<Commande?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Commande>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Commande commande, CancellationToken cancellationToken = default);
    Task DeleteAsync(Commande commande, CancellationToken cancellationToken = default);
    Task<Commande?> GetCommandeByStatutAsync(int clientId, StatutCommande statut, CancellationToken cancellationToken = default);
    Task<decimal> GetPrixProduitAsync(int produitId, CancellationToken cancellationToken = default);
}