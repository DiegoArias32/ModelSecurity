// Business/BaseBusiness.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace Business
{
    public class BaseBusiness<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        protected readonly IService<TEntity, TDto> _service;
        protected readonly ILogger _logger;

        public BaseBusiness(IService<TEntity, TDto> service, ILogger logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            try
            {
                return await _service.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los elementos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de elementos", ex);
            }
        }

        public virtual async Task<TDto> GetByIdAsync(object id)
        {
            try
            {
                var entity = await _service.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }
                return entity;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el elemento con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el elemento con ID {id}", ex);
            }
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            try
            {
                return await _service.CreateAsync(dto);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear elemento");
                throw new ExternalServiceException("Base de datos", "Error al crear el elemento", ex);
            }
        }

        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            try
            {
                var success = await _service.UpdateAsync(dto);
                if (!success)
                {
                    throw new EntityNotFoundException(typeof(TEntity).Name, GetId(dto));
                }
                return dto;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar elemento");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el elemento", ex);
            }
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            try
            {
                return await _service.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar elemento con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el elemento", ex);
            }
        }

        public virtual async Task<bool> PermanentDeleteAsync(object id)
        {
            try
            {
                return await _service.PermanentDeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente elemento con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el elemento", ex);
            }
        }

        // Helper method to get the ID from the DTO
        protected virtual object GetId(TDto dto)
        {
            var idProperty = typeof(TDto).GetProperty("Id");
            if (idProperty != null)
            {
                return idProperty.GetValue(dto);
            }

            var alternativeIdProperties = new[] { "RolId", "FormId", "ModuleId", "UserId", "LoginId", "WorkerId" };
            foreach (var propName in alternativeIdProperties)
            {
                var prop = typeof(TDto).GetProperty(propName);
                if (prop != null)
                {
                    return prop.GetValue(dto);
                }
            }

            throw new InvalidOperationException("No se pudo determinar el ID del DTO");
        }
    }
}