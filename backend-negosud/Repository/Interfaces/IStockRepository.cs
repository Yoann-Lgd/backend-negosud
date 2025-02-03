using backend_negosud.Entities;

namespace backend_negosud.Repository;

public interface IStockRepository : IRepositoryBase<Stock>
{
    Task<List<Stock>> GetStocksAReapprovisionnerAsync();
}