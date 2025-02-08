using backend_negosud.DTOs.Famille;
using backend_negosud.DTOs.Famille.Outputs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IFamilleService
{
    Task<IResponseDataModel<List<FamilleOutputDto>>> GetAllFamilles();
    Task<IResponseDataModel<FamilleOutputDto>> getFamilleById(int id);
    Task<IResponseDataModel<string>> CreateFamille(FamilleCreateInputDto familleDto);
    
}