using System;
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
    public class PermissionService : Service<Permission, PermissionDto>
    {
        private readonly IValidationStrategy<PermissionDto> _validationStrategy;

        public PermissionService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<Permission, PermissionDto>> logger,
            IValidationStrategy<PermissionDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<PermissionDto> CreateAsync(PermissionDto dto)
        {
            // Validación básica - aseguramos que al menos tiene un permiso
            if (!dto.CanRead && !dto.CanCreate && !dto.CanUpdate && !dto.CanDelete)
            {
                throw new ValidationException("Permissions", "El permiso debe tener al menos un privilegio habilitado (Leer, Crear, Actualizar o Eliminar).");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(PermissionDto dto)
        {
            // Validación básica - aseguramos que al menos tiene un permiso
            if (!dto.CanRead && !dto.CanCreate && !dto.CanUpdate && !dto.CanDelete)
            {
                throw new ValidationException("Permissions", "El permiso debe tener al menos un privilegio habilitado (Leer, Crear, Actualizar o Eliminar).");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.UpdateAsync(dto);
        }
    }
}