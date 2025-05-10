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
    public class RolFormPermissionService : Service<RolFormPermission, RolFormPermissionDto>
    {
        private readonly IValidationStrategy<RolFormPermissionDto> _validationStrategy;

        public RolFormPermissionService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<RolFormPermission, RolFormPermissionDto>> logger,
            IValidationStrategy<RolFormPermissionDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<RolFormPermissionDto> CreateAsync(RolFormPermissionDto dto)
        {
            // Validación básica
            if (dto.RolId <= 0)
            {
                throw new ValidationException("RolId", "El ID de rol debe ser mayor que cero.");
            }

            if (dto.FormId <= 0)
            {
                throw new ValidationException("FormId", "El ID de formulario debe ser mayor que cero.");
            }

            if (dto.PermissionId <= 0)
            {
                throw new ValidationException("PermissionId", "El ID de permiso debe ser mayor que cero.");
            }

            // Verificar que el rol existe
            var rolRepo = _unitOfWork.GetRepository<Rol>();
            var rol = await rolRepo.GetByIdAsync(dto.RolId);
            if (rol == null)
            {
                throw new ValidationException("RolId", $"No existe un rol con ID {dto.RolId}");
            }

            // Verificar que el formulario existe
            var formRepo = _unitOfWork.GetRepository<Form>();
            var form = await formRepo.GetByIdAsync(dto.FormId);
            if (form == null)
            {
                throw new ValidationException("FormId", $"No existe un formulario con ID {dto.FormId}");
            }

            // Verificar que el permiso existe
            var permissionRepo = _unitOfWork.GetRepository<Permission>();
            var permission = await permissionRepo.GetByIdAsync(dto.PermissionId);
            if (permission == null)
            {
                throw new ValidationException("PermissionId", $"No existe un permiso con ID {dto.PermissionId}");
            }

            // Verificar que la asignación no existe ya
            var rfps = await _unitOfWork.GetRepository<RolFormPermission>().GetAllAsync();
            if (rfps.Any(rfp => rfp.RolId == dto.RolId && rfp.FormId == dto.FormId && rfp.PermissionId == dto.PermissionId))
            {
                throw new ValidationException("RolFormPermission", 
                    $"Ya existe una asignación para el rol con ID {dto.RolId}, formulario con ID {dto.FormId} y permiso con ID {dto.PermissionId}");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(RolFormPermissionDto dto)
        {
            // Validación básica
            if (dto.RolId <= 0)
            {
                throw new ValidationException("RolId", "El ID de rol debe ser mayor que cero.");
            }

            if (dto.FormId <= 0)
            {
                throw new ValidationException("FormId", "El ID de formulario debe ser mayor que cero.");
            }

            if (dto.PermissionId <= 0)
            {
                throw new ValidationException("PermissionId", "El ID de permiso debe ser mayor que cero.");
            }

            // Verificar que el rol existe
            var rolRepo = _unitOfWork.GetRepository<Rol>();
            var rol = await rolRepo.GetByIdAsync(dto.RolId);
            if (rol == null)
            {
                throw new ValidationException("RolId", $"No existe un rol con ID {dto.RolId}");
            }

            // Verificar que el formulario existe
            var formRepo = _unitOfWork.GetRepository<Form>();
            var form = await formRepo.GetByIdAsync(dto.FormId);
            if (form == null)
            {
                throw new ValidationException("FormId", $"No existe un formulario con ID {dto.FormId}");
            }

            // Verificar que el permiso existe
            var permissionRepo = _unitOfWork.GetRepository<Permission>();
            var permission = await permissionRepo.GetByIdAsync(dto.PermissionId);
            if (permission == null)
            {
                throw new ValidationException("PermissionId", $"No existe un permiso con ID {dto.PermissionId}");
            }

            // Verificar que la asignación no existe ya (excluyendo la actual)
            var rfps = await _unitOfWork.GetRepository<RolFormPermission>().GetAllAsync();
            if (rfps.Any(rfp => rfp.RolId == dto.RolId && rfp.FormId == dto.FormId && 
                                rfp.PermissionId == dto.PermissionId && rfp.Id != dto.Id))
            {
                throw new ValidationException("RolFormPermission", 
                    $"Ya existe una asignación para el rol con ID {dto.RolId}, formulario con ID {dto.FormId} y permiso con ID {dto.PermissionId}");
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