using backend_negosud.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using backend_negosud.Models;

namespace backend_negosud.Repository
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly PostgresContext _context;

        public RepositoryBase(PostgresContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<TEntity>().Add(entity);
                await SaveChangesAsync(cancellationToken);
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur pendant l'ajout de l'entité.", ex);
            }
        }
        
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,  CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        
        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Set<TEntity>().ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur pendant la récupérations des entités.", ex);
            }
        }
        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
        }
        
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<TEntity>().Update(entity);
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur pendant la mis à jour", ex);
            }
        }
        
        public virtual async Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            try
            {
                return await _context.FindAsync<TEntity>(id, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération avec l'id : {id}.", ex);
            }
        }
        
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<TEntity>().Remove(entity);
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur pendant la suppression de l'entité", ex);
            }
        }
        
        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur pendant le sauvegarde en base de donnése.", ex);
            }
        }
        
        public virtual async Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Set<TEntity>().ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur qui concerne le listing des entités", ex);
            }
        }
        
        public async Task<bool>  SoftDeleteEntityByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : class, ISoftDelete
            where TId : notnull
        {
            try
            {
                var entity = await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);

                if (entity != null)
                {
                    entity.DeletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                    return true;
                }
                else
                {
                    throw new KeyNotFoundException($"L'entité avec l'{id} est inconnue.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la suppression de l'entité.", ex);
            }
        }

    }
}
