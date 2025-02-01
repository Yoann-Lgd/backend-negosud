using backend_negosud.Entities;
using backend_negosud.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
                throw new Exception("Error adding entity.", ex);
            }
        }
        
        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Set<TEntity>().ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving entities.", ex);
            }
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
                throw new Exception("Error updating entity.", ex);
            }
        }
        
        public virtual async Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            try
            {
                return await _context.FindAsync<TEntity>([id], cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving entity with ID {id}.", ex);
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
                throw new Exception("Error deleting entity.", ex);
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
                throw new Exception("Error saving changes to the database.", ex);
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
                throw new Exception("Error listing entities.", ex);
            }
        }
    }
}
