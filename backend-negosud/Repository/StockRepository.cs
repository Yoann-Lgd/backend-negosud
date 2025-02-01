using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class StockRepository : RepositoryBase<Stock>, IStockRepository
{

    public StockRepository(PostgresContext context) : base(context)
    {
    }
    
}