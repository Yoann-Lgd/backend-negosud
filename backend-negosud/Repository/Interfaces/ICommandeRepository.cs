using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Enums;

namespace backend_negosud.Repository;

public interface ICommandeRepository : IRepositoryBase<Commande, CommandeOutputDto>
{
    Task<Commande?> GetCommandeByStatut(int clientId, StatutCommande statut);
    Task AddAsync(Commande commande);
    Task UpdateAsync(Commande commande);
    Task<decimal> GetPrixProduit(int produitId);
}