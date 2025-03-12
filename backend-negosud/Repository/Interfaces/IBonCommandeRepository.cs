using backend_negosud.Entities;

namespace backend_negosud.Repository;

public interface IBonCommandeRepository : IRepositoryBase<BonCommande>
{
    Task<List<BonCommande>> GetAllCommandeAsync();
    Task<BonCommande> GetById(int id);
}