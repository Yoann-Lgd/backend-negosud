using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface ICommandeService
{
    Task<IResponseDataModel<List<CommandeOutputDto>>> GetAllCommandes();
    Task<CommandeOutputDto> GetCommandeById(int id);
}
