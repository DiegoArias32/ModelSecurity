using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Utilities.Interfaces;

namespace Utilities.Services
{
    public class Service<TEntity, TDto> : IService<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<Service<TEntity, TDto>> _logger;
        protected readonly IRepository<TEntity> _repository;

        public Service(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Service<TEntity, TDto>> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = _unitOfWork.GetRepository<TEntity>();
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            try
            {
                // Map DTO to entity
                var entity = _mapper.Map<TEntity>(dto);
                
                // Create entity
                var createdEntity = await _repository.CreateAsync(entity);
                
                // Map entity back to DTO
                return _mapper.Map<TDto>(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el elemento");
                throw;
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
                _logger.LogError(ex, "Error al eliminar el elemento con ID {Id}", id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<TDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los elementos");
                throw;
            }
        }

        public virtual async Task<TDto> GetByIdAsync(object id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                return _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el elemento con ID {Id}", id);
                throw;
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
                _logger.LogError(ex, "Error al eliminar permanentemente el elemento con ID {Id}", id);
                throw;
            }
        }

        public virtual async Task<bool> UpdateAsync(TDto dto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(dto);
                return await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el elemento");
                throw;
            }
        }
    }
}