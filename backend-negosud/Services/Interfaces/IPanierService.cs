using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IPanierService
{
    Task<IResponseDataModel<PanierCreateOutputDto>> CreatePanier(PanierCreateInputDto panierCreateInputDto);
}