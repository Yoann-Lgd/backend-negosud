using backend_negosud.DTOs;
using backend_negosud.Entities;

namespace backend_negosud.Repository;

public interface IClientRepository : IRepositoryBase<Client>
{
    // TODO faire une interface qui mutualise cette methode
    Task<bool> EmailExistsAsync(string email);

    Task<Client?> GetClientBydIdComandes(int id);
}