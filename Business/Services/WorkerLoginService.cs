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
    public class WorkerLoginService : Service<WorkerLogin, WorkerLoginDto>
    {
        private readonly IValidationStrategy<WorkerLoginDto> _validationStrategy;

        public WorkerLoginService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<WorkerLogin, WorkerLoginDto>> logger,
            IValidationStrategy<WorkerLoginDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<WorkerLoginDto> CreateAsync(WorkerLoginDto dto)
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

            if (dto.WorkerId <= 0)
            {
                throw new ValidationException("WorkerId", "El ID del trabajador debe ser mayor que cero.");
            }

            if (dto.LoginId <= 0)
            {
                throw new ValidationException("LoginId", "El ID del login debe ser mayor que cero.");
            }

            // Verificar que el trabajador existe
            var workerRepo = _unitOfWork.GetRepository<Worker>();
            var worker = await workerRepo.GetByIdAsync(dto.WorkerId);
            if (worker == null)
            {
                throw new ValidationException("WorkerId", $"No existe un trabajador con ID {dto.WorkerId}");
            }

            // Verificar que el login existe
            var loginRepo = _unitOfWork.GetRepository<Login>();
            var login = await loginRepo.GetByIdAsync(dto.LoginId);
            if (login == null)
            {
                throw new ValidationException("LoginId", $"No existe un login con ID {dto.LoginId}");
            }

            // Verificar nombre de usuario único
            var workerLogins = await _unitOfWork.GetRepository<WorkerLogin>().GetAllAsync();
            if (workerLogins.Any(wl => wl.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Username", $"Ya existe un login de trabajador con el nombre de usuario '{dto.Username}'");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            // Establecer valores predeterminados
            dto.Status = true; // Por defecto, el login está activo

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(WorkerLoginDto dto)
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

            if (dto.WorkerId <= 0)
            {
                throw new ValidationException("WorkerId", "El ID del trabajador debe ser mayor que cero.");
            }

            if (dto.LoginId <= 0)
            {
                throw new ValidationException("LoginId", "El ID del login debe ser mayor que cero.");
            }

            // Verificar que el trabajador existe
            var workerRepo = _unitOfWork.GetRepository<Worker>();
            var worker = await workerRepo.GetByIdAsync(dto.WorkerId);
            if (worker == null)
            {
                throw new ValidationException("WorkerId", $"No existe un trabajador con ID {dto.WorkerId}");
            }

            // Verificar que el login existe
            var loginRepo = _unitOfWork.GetRepository<Login>();
            var login = await loginRepo.GetByIdAsync(dto.LoginId);
            if (login == null)
            {
                throw new ValidationException("LoginId", $"No existe un login con ID {dto.LoginId}");
            }

            // Verificar nombre de usuario único (excluyendo el login actual)
            var workerLogins = await _unitOfWork.GetRepository<WorkerLogin>().GetAllAsync();
            if (workerLogins.Any(wl => wl.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase) && wl.id != dto.id))
            {
                throw new ValidationException("Username", $"Ya existe un login de trabajador con el nombre de usuario '{dto.Username}'");
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