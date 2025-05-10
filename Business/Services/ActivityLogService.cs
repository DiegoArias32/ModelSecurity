using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Interfaces;
using Utilities.Services;

namespace Business.Services
{
    public class ActivityLogService : Service<ActivityLog, ActivityLogDto>
    {
        private readonly IValidationStrategy<ActivityLogDto> _validationStrategy;

        public ActivityLogService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<ActivityLog, ActivityLogDto>> logger,
            IValidationStrategy<ActivityLogDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<ActivityLogDto> CreateAsync(ActivityLogDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                throw new ValidationException("UserId", "El ID de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                throw new ValidationException("UserName", "El nombre de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Action))
            {
                throw new ValidationException("Action", "La acción es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(dto.EntityType))
            {
                throw new ValidationException("EntityType", "El tipo de entidad es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.EntityId))
            {
                throw new ValidationException("EntityId", "El ID de entidad es obligatorio.");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            // Establecer valores predeterminados
            dto.Timestamp = DateTime.UtcNow;

            return await base.CreateAsync(dto);
        }

        // La actualización no tiene sentido para un registro de actividad
        // que es inmutable por naturaleza, pero implementamos la validación igualmente
        public override async Task<bool> UpdateAsync(ActivityLogDto dto)
        {
            // Lanzar una excepción ya que no se deben actualizar los registros de actividad
            throw new InvalidOperationException("Los registros de actividad no se pueden actualizar. Son inmutables.");
        }

        // El borrado no debería ser permitido para registros de auditoría
        public override async Task<bool> DeleteAsync(object id)
        {
            // Lanzar una excepción ya que no se deben eliminar los registros de actividad
            throw new InvalidOperationException("Los registros de actividad no se pueden eliminar. Son inmutables.");
        }

        // El borrado permanente debería restringirse aún más
        public override async Task<bool> PermanentDeleteAsync(object id)
        {
            // Permitimos esto solo para administradores con ciertos privilegios
            // En una implementación real, esto debería verificar permisos de usuario
            
            _logger.LogWarning("ADVERTENCIA: Se está intentando eliminar permanentemente un registro de actividad con ID {ActivityLogId}", id);
            
            return await base.PermanentDeleteAsync(id);
        }
    }
}