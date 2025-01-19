using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Repository;

public interface IUtilisateurRepository : IRepositoryBase<Utilisateur , UtilisateurOutputDto>
{
    /*Task<IResponseDataModel<UtilisateurOutputDto>> CreateAsync(UtilisateurInputDto UtilisateurInputDto);*/

    Task<bool> EmailExistsAsync(string email);
}