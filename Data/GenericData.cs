// Archivo: Data/GenericData.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;

namespace Data
{
    // Clase base que implementa la interfaz
    public class GenericData<TEntity> : IGenericData<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger _logger;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericData(ApplicationDbContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            try
            {
                var propInfo = typeof(TEntity).GetProperty("CreateAt");
                if (propInfo != null && propInfo.PropertyType == typeof(DateTime))
                    propInfo.SetValue(entity, DateTime.UtcNow);

                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear: {Message}", ex.Message);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar: {Message}", ex.Message);
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null) return false;

                var propInfo = typeof(TEntity).GetProperty("DeleteAt");
                if (propInfo != null && propInfo.PropertyType == typeof(DateTime?))
                {
                    propInfo.SetValue(entity, DateTime.UtcNow);
                    _dbSet.Update(entity);
                }
                else
                    _dbSet.Remove(entity);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar: {Message}", ex.Message);
                return false;
            }
        }

        public virtual async Task<bool> PermanentDeleteAsync(object id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null) return false;

                _dbSet.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente: {Message}", ex.Message);
                return false;
            }
        }
    }
}