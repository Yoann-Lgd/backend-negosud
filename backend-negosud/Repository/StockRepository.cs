using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class StockRepository : IStockRepository
{
    protected readonly PostgresContext _context;

    public StockRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<ResponseDataModel<Stock>> AddAsync(Stock entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Stock>().Add(entity);
        await SaveChangesAsync(cancellationToken);
        return new ResponseDataModel<Stock>
        {
            Data = entity
        };
    }

    public async Task<ResponseDataModel<Stock>> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        var entity = await _context.FindAsync<Stock>(new object[] { id }, cancellationToken);
        return new ResponseDataModel<Stock>
        {
            Data = entity
        };
    }

    public async Task<ResponseDataModel<List<Stock>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await ListAsync(cancellationToken);
        return new ResponseDataModel<List<Stock>>
        {
            Data = entities
        };
    }

    public async Task UpdateAsync(Stock entity, CancellationToken cancellationToken = default)
    {
        var existingEntity = await _context.FindAsync<Stock>(new object[] { entity.StockId }, cancellationToken);
        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Stock entity, CancellationToken cancellationToken = default)
    {
        _context.Set<Stock>().Remove(entity);
        await SaveChangesAsync(cancellationToken);
    }

    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<List<Stock>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Stock>().ToListAsync(cancellationToken);
    }
}