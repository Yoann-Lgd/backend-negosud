using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;

namespace backend_negosud.Repository;

public interface IUtilisateurRepository : IRepositoryBase<Utilisateur>
{
    /*Task<IResponseDataModel<UtilisateurOutputDto>> CreateAsync(UtilisateurInputDto UtilisateurInputDto);*/
    // TODO faire une interface qui mutualise cette methode
    Task<bool> EmailExistsAsync(string email);
    Task<List<Utilisateur>> GetUtilisateursWithRole();
}