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
    public class RolUserService : Service<RolUser, RolUserDto>
    {
        private readonly IValidationStrategy<RolUserDto> _validationStrategy;

        public RolUserService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<RolUser, RolUserDto>> logger,
            IValidationStrategy<RolUserDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<RolUserDto> CreateAsync(RolUserDto dto)
        {
            // Validación básica
            if (dto.UserId <= 0)
            {
                throw new ValidationException("UserId", "El ID de usuario debe ser mayor que cero.");
            }

            if (dto.RolId <= 0)
            {
                throw new ValidationException("RolId", "El ID de rol debe ser mayor que cero.");
            }

            // Verificar que el usuario existe
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new ValidationException("UserId", $"No existe un usuario con ID {dto.UserId}");
            }

            // Verificar que el rol existe
            var rolRepo = _unitOfWork.GetRepository<Rol>();
            var rol = await rolRepo.GetByIdAsync(dto.RolId);
            if (rol == null)
            {
                throw new ValidationException("RolId", $"No existe un rol con ID {dto.RolId}");
            }

            // Verificar que la asignación no existe ya
            var rolUsers = await _unitOfWork.GetRepository<RolUser>().GetAllAsync();
            if (rolUsers.Any(ru => ru.UserId == dto.UserId && ru.RolId == dto.RolId))
            {
                throw new ValidationException("RolUser", $"El usuario con ID {dto.UserId} ya tiene asignado el rol con ID {dto.RolId}");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(RolUserDto dto)
        {
            // Validación básica
            if (dto.UserId <= 0)
            {
                throw new ValidationException("UserId", "El ID de usuario debe ser mayor que cero.");
            }

            if (dto.RolId <= 0)
            {
                throw new ValidationException("RolId", "El ID de rol debe ser mayor que cero.");
            }

            // Verificar que el usuario existe
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new ValidationException("UserId", $"No existe un usuario con ID {dto.UserId}");
            }

            // Verificar que el rol existe
            var rolRepo = _unitOfWork.GetRepository<Rol>();
            var rol = await rolRepo.GetByIdAsync(dto.RolId);
            if (rol == null)
            {
                throw new ValidationException("RolId", $"No existe un rol con ID {dto.RolId}");
            }

            // Verificar que la asignación no existe ya (excluyendo la actual)
            var rolUsers = await _unitOfWork.GetRepository<RolUser>().GetAllAsync();
            if (rolUsers.Any(ru => ru.UserId == dto.UserId && ru.RolId == dto.RolId && ru.Id != dto.Id))
            {
                throw new ValidationException("RolUser", $"El usuario con ID {dto.UserId} ya tiene asignado el rol con ID {dto.RolId}");
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