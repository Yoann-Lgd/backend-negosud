using backend_negosud.Entities;
using backend_negosud.Enums;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface ICommandeService
{
    Task<IResponseDataModel<Commande>> CreateCommandeAsync(int clientId);
    Task<IResponseDataModel<string>> AddLigneCommandeAsync(int commandeId, int produitId, int quantite, decimal prix);
    Task<IResponseDataModel<string>> ValidateCommandeAsync(int commandeId);
    Task<IResponseDataModel<string>> CancelCommandeAsync(int commandeId);
    Task<IResponseDataModel<Commande>> GetCommandeByIdAsync(int commandeId);
    Task<IResponseDataModel<List<Commande>>> GetCommandesByStatutAsync(StatutCommande statut);
}