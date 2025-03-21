using backend_negosud.DTOs;
using backend_negosud.DTOs.Utilisateur.Input;
using backend_negosud.Entities;
using backend_negosud.Models;
using Supabase.Gotrue;

namespace backend_negosud.Services;

public interface IUtilisateurService : IAuthService<Utilisateur, UtilisateurInputDto, UtilisateurOutputDto>
{
    Task<IResponseDataModel<UtilisateurOutputDto>> CreateUtilisateur(UtilisateurInputDto utilisateurInputDto);

    Task<Utilisateur> GetUtilisateurByToken(string token);

    Task<IResponseDataModel<string>> UpdateUtilisateur(int id, UtilisateurInputDtoWithRole utilisateurInputDto);

    Task <Utilisateur>GetUtilisateuByEmail(string email);
    Task<BooleanResponseDataModel> UtilisateurExistEmail(UtilisateurEmailInputDto utilisateurEmailInputDto);

    Task<IResponseDataModel<UtilisateurOutputDto>> Login(string email, string motDePasse);
    
    Task<IResponseDataModel<UtilisateurOutputDto>> GetUtilisateurById(int id);
    
    Task<IResponseDataModel<List<UtilisateurOutputDto>>> GetAllUtilisateurs();

    Task<IResponseDataModel<string>> ResetMotDePasse(string email);
    Task<IResponseDataModel<string>> SoftDeleteAsync(int id);
}