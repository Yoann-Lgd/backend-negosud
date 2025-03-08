using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class StockRepository : RepositoryBase<Stock>, IStockRepository
{

    public StockRepository(PostgresContext context) : base(context)
    {
    }
    
    public async Task<List<Stock>> GetStocksAReapprovisionnerAsync()
    {
        return await _context.Stocks
            .Where(s => s.Quantite <= s.SeuilMinimum && s.ReapprovisionnementAuto)
            .ToListAsync();
    }

    public async Task<List<Stock>> GetAllStocksWithArticles()
    {
        return await _context.Stocks
            .Include(a => a.Article)
            .ToListAsync();
    }
}