using backend_negosud.entities;
using backend_negosud.Models;

namespace backend_negosud.Services;

public interface IAuthService
{
    public Task<IResponseDataModel<Utilisateur>> Login(string email, string motDePasse);

    public Task<IResponseDataModel<string>> ResetMotDePasse(string email);
}