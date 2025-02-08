using backend_negosud.DTOs.Commande_client;
using backend_negosud.DTOs.Commande_client.Outputs;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IPanierService
{
    Task<IResponseDataModel<PanierOutputDto>> CreatePanier(PanierInputDto panierInputDto);
    Task<IResponseDataModel<PanierOutputDto>> UpdatePanier(PanierUpdateInputDto panierInputDto);

    Task<IResponseDataModel<string>> DeletePanier(int id);
    Task<IResponseDataModel<PanierOutputDto>> GetBasketByClientId(int id);
    Task<IResponseDataModel<string>> ExtendDurationBasket(int id);
    Task<IResponseDataModel<CommandeOutputDto>> BasketToCommand(int id);
}