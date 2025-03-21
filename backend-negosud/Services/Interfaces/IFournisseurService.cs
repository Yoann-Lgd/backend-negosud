using backend_negosud.DTOs.Fournisseur.FournisseurInputDto;
using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IFournisseurService
{
    Task<IResponseDataModel<List<FournisseurOutputCompleteDto>>> getAll();
    Task<IResponseDataModel<string>> softDeleteFournisseurById(int id);
    Task<IResponseDataModel<string>> PatchMinimalFournisseur(int id, FournisseurInputMinimal fournisseurInputMinimal);
    Task<IResponseDataModel<string>> CreateFournisseur(FournisseurInputMinimal fournisseurInputMinimal);

    Task<IResponseDataModel<FournisseurOutputCompleteDto>> GetFournisseurById(int id);

}