using backend_negosud.DTOs;
using backend_negosud.entities;
using backend_negosud.Models;
using Supabase.Gotrue;

namespace backend_negosud.Services;

public interface IUtilisateurService
{
    Task<IResponseModel> CreateUtilisateur(UtilisateurInputDto utilisateur);

    Task<Utilisateur> GetUtilisateurByToken(string token);

    Task UpdateUtilisateur(Utilisateur utilisateur);

    Task <Utilisateur>GetUtilisateuByEmail(string email);
}