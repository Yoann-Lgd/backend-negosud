using backend_negosud.DTOs;
using backend_negosud.Models;

namespace backend_negosud.Repository;

public interface IUtilisateurRepository
{
    Task<IResponseDataModel<UtilisateurOutputDto>> CreateAsync(UtilisateurInputDto UtilisateurInputDto);
}