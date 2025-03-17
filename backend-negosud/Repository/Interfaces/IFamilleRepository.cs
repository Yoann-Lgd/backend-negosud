using backend_negosud.Entities;

namespace backend_negosud.Repository.Interfaces;

public interface IFamilleRepository : IRepositoryBase<Famille>
{
    Task<List<Famille>> GetAllFamillesAsync();
    Task<Famille> GetFamilleByIdAndArticles(int id);
}