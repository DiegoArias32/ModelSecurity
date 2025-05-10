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
    public class LoginService : Service<Login, LoginDto>
    {
        private readonly IValidationStrategy<LoginDto> _validationStrategy;

        public LoginService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<Login, LoginDto>> logger,
            IValidationStrategy<LoginDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<LoginDto> CreateAsync(LoginDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                throw new ValidationException("Username", "El nombre de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ValidationException("Password", "La contraseña es obligatoria.");
            }

            // Verificar nombre de usuario único
            var logins = await _unitOfWork.GetRepository<Login>().GetAllAsync();
            if (logins.Any(l => l.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Username", $"Ya existe un login con el nombre de usuario '{dto.Username}'");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            // Aquí podríamos agregar seguridad adicional (hash de contraseña, etc.)
            // Esto debería hacerse en una implementación real

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(LoginDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                throw new ValidationException("Username", "El nombre de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ValidationException("Password", "La contraseña es obligatoria.");
            }

            // Verificar nombre de usuario único (excluyendo el login actual)
            var logins = await _unitOfWork.GetRepository<Login>().GetAllAsync();
            if (logins.Any(l => l.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase) && l.LoginId != dto.LoginId))
            {
                throw new ValidationException("Username", $"Ya existe un login con el nombre de usuario '{dto.Username}'");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            // Aquí podríamos agregar seguridad adicional (hash de contraseña, etc.)
            // Esto debería hacerse en una implementación real

            return await base.UpdateAsync(dto);
        }
    }
}