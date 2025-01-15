using backend_negosud.DTOs;
using backend_negosud.entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IUtilisateurService
{
    Task<IResponseModel> CreateUtilisateur(UtilisateurInputDto utilisateur);
}