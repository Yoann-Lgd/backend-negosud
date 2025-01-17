using backend_negosud.entities;

namespace backend_negosud.Services;

public interface IJwtService
{
    string GenererToken(Utilisateur utilisateur);
    Task<bool> ValidateToken(string token, int id);
}