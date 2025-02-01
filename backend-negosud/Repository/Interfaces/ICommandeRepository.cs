using backend_negosud.Entities;

namespace backend_negosud.Repository;

public interface ICommandeRepository : IRepositoryBase<Commande>
{
    Task<List<Commande>> GetAllCommandeAsync();
}