using backend_negosud.Entities;
using backend_negosud.Repository;

namespace backend_negosud.Services;

public interface IInventorierRepository : IRepositoryBase<Inventorier>
{
    Task<List<Inventorier>> GetInventoriersByStockIdAsync(int stockId, CancellationToken cancellationToken = default);
}