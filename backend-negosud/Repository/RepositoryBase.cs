using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Repository;

public class RepositoryBase<TEntity, T> : IRepositoryBase<TEntity, T> where TEntity : class where T : class
{
    protected readonly PostgresContext _context;

    public RepositoryBase(PostgresContext context)
    {
        _context = context;
    }

public virtual async Task<ResponseDataModel<T>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
{
    _context.Set<TEntity>().Add(entity);
    await SaveChangesAsync(cancellationToken);
    return new ResponseDataModel<T>
    {
        Data = entity as T
    };
}
    
    public virtual async Task<ResponseDataModel<List<T>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await ListAsync(cancellationToken);
        return new ResponseDataModel<List<T>>
        {
            Data = entities.Cast<T>().ToList()
        };
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Update(entity);
        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<ResponseDataModel<T>> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        var entity = await _context.FindAsync<TEntity>(new object[] { id }, cancellationToken);
        return new ResponseDataModel<T>
        {
            Data = entity as T
        };
    }
    
    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Remove(entity);
        await SaveChangesAsync(cancellationToken);
    }

    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }


    public virtual async Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().ToListAsync(cancellationToken);
    }
}