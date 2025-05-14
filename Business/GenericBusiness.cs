using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Data;

namespace Business
{
    public interface IGenericBusiness<TDto, TEntity>
        where TDto : class
        where TEntity : class
    {
        Task<TDto> CreateAsync(TDto dto);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(object id);
        Task<bool> UpdateAsync(TDto dto);
        Task<bool> DeleteAsync(object id);
        Task<bool> PermanentDeleteAsync(object id);
    }

    public abstract class GenericBusiness<TDto, TEntity> : IGenericBusiness<TDto, TEntity>
        where TDto : class
        where TEntity : class
    {
        protected readonly IGenericData<TEntity> _data;
        protected readonly ILogger<GenericBusiness<TDto, TEntity>> _logger;

        public GenericBusiness(IGenericData<TEntity> data, ILogger<GenericBusiness<TDto, TEntity>> logger)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            try
            {
                ValidateDto(dto);
                var entity = MapToEntity(dto);
                var createdEntity = await _data.CreateAsync(entity);
                return MapToDto(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            try
            {
                var entities = await _data.GetAllAsync();
                return MapToDtoList(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public virtual async Task<TDto> GetByIdAsync(object id)
        {
            try
            {
                var entity = await _data.GetByIdAsync(id);
                if (entity == null)
                    return null;

                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener por ID {Id}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }

        public virtual async Task<bool> UpdateAsync(TDto dto)
        {
            try
            {
                ValidateDto(dto);
                var entity = MapToEntity(dto);
                return await _data.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            try
            {
                return await _data.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar con ID {Id}: {ErrorMessage}", id, ex.Message);
                return false;
            }
        }

        public virtual async Task<bool> PermanentDeleteAsync(object id)
        {
            try
            {
                return await _data.PermanentDeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente con ID {Id}: {ErrorMessage}", id, ex.Message);
                return false;
            }
        }

        // Métodos abstractos que deben ser implementados en las clases derivadas
        protected abstract void ValidateDto(TDto dto);
        protected abstract TEntity MapToEntity(TDto dto);
        protected abstract TDto MapToDto(TEntity entity);
        protected abstract IEnumerable<TDto> MapToDtoList(IEnumerable<TEntity> entities);
    }
}