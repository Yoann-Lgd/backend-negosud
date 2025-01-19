using backend_negosud.DTOs;
using backend_negosud.Entities;
using backend_negosud.Models;
using Supabase.Gotrue;

namespace backend_negosud.Services;

public interface IUtilisateurService
{
    Task<IResponseDataModel<UtilisateurOutputDto>> CreateUtilisateur(UtilisateurInputDto utilisateurInputDto);

    Task<Utilisateur> GetUtilisateurByToken(string token);

    Task UpdateUtilisateur(Utilisateur utilisateur);

    Task <Utilisateur>GetUtilisateuByEmail(string email);
}