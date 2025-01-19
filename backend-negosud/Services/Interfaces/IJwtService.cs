using backend_negosud.Entities;

namespace backend_negosud.Services;

public interface IJwtService<T> where T : class
{
    string GenererToken(T generic);
    Task<bool> ValidateToken(string token, int id);
}