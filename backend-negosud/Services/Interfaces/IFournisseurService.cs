using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IFournisseurService
{
    Task<IResponseDataModel<List<FournisseurOutputCompleteDto>>> getAll();
}