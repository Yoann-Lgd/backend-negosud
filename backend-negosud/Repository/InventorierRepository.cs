using AutoMapper;
using backend_negosud.Entities;
using backend_negosud.Services;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class InventorierRepository : RepositoryBase<Inventorier>, IInventorierRepository
{
    
    public InventorierRepository(PostgresContext context):base(context)
    {
    }
    
    public async Task<List<Inventorier>> GetInventoriersByStockIdAsync(int stockId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventoriers
            .Where(i => i.StockId == stockId)
            .OrderByDescending(i => i.DateModification)
            .ToListAsync(cancellationToken);
    }
}