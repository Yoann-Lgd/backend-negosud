using backend_negosud.DTOs;
using backend_negosud.Models;

namespace backend_negosud.Repository;

public interface IRepositoryBase<TEntity, T> where TEntity : class where T : class
{
    Task<ResponseDataModel<T>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseDataModel<T>>  GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    Task<ResponseDataModel<List<T>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}

