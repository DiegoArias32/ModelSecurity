// Data/BaseData.cs
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class BaseData<T> where T : class
    {
        protected readonly IRepository<T> _repository;
        protected readonly ILogger _logger;

        public BaseData(IRepository<T> repository, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            try
            {
                return await _repository.CreateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear entidad");
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las entidades");
                throw;
            }
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entidad por ID: {Id}", id);
                throw;
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                return await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar entidad");
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar entidad con ID: {Id}", id);
                return false;
            }
        }

        public virtual async Task<bool> PermanentDeleteAsync(object id)
        {
            try
            {
                return await _repository.PermanentDeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente entidad con ID: {Id}", id);
                return false;
            }
        }
    }
}