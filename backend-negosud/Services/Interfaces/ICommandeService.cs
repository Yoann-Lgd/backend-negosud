using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface ICommandeService
{
    Task<IResponseDataModel<List<CommandeOutputDto>>> GetAllCommandes();

    Task<BooleanResponseDataModel> UpdateOrderStatusToPaidAsync(string orderId, decimal? stripeAmount = null);
    Task<CommandeOutputDto> GetCommandeById(int id);
    Task<IResponseDataModel<CommandeOutputDto>> CreateCommande(CommandeInputDto commandeInput);
    Task<IResponseDataModel<string>> SoftDeleteAsync(int commandeId);
}
