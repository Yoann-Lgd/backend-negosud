using backend_negosud.DTOs.Commande_fournisseur.Inputs;
using backend_negosud.DTOs.Commande_fournisseur.Outputs;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IBonCommandeService
{
    Task<BooleanResponseDataModel> CreateBonCommande(BonCommandeCreateInputDto bonCommandeInput);
    Task<IResponseDataModel<List<BonCommandeOutputDto>>> GetAllBonCommandes();
    Task<IResponseDataModel<BonCommandeOutputDto>> GetBonCommandeById(int id);
}