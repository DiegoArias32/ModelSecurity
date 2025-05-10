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
    public class WorkerService : Service<Worker, WorkerDto>
    {
        private readonly IValidationStrategy<WorkerDto> _validationStrategy;

        public WorkerService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<Worker, WorkerDto>> logger,
            IValidationStrategy<WorkerDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<WorkerDto> CreateAsync(WorkerDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                throw new ValidationException("FirstName", "El nombre del trabajador es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                throw new ValidationException("LastName", "El apellido del trabajador es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.IdentityDocument))
            {
                throw new ValidationException("IdentityDocument", "El documento de identidad del trabajador es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ValidationException("Email", "El correo electrónico del trabajador es obligatorio.");
            }

            // Verificar documento único
            var workers = await _unitOfWork.GetRepository<Worker>().GetAllAsync();
            if (workers.Any(w => w.IdentityDocument == dto.IdentityDocument))
            {
                throw new ValidationException("IdentityDocument", $"Ya existe un trabajador con el documento '{dto.IdentityDocument}'");
            }

            // Verificar email único
            if (workers.Any(w => w.Email == dto.Email))
            {
                throw new ValidationException("Email", $"Ya existe un trabajador con el correo electrónico '{dto.Email}'");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            // Establecer fecha de contratación por defecto si no se proporciona
            if (dto.HireDate == null)
            {
                dto.HireDate = DateTime.Now;
            }

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(WorkerDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                throw new ValidationException("FirstName", "El nombre del trabajador es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                throw new ValidationException("LastName", "El apellido del trabajador es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.IdentityDocument))
            {
                throw new ValidationException("IdentityDocument", "El documento de identidad del trabajador es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ValidationException("Email", "El correo electrónico del trabajador es obligatorio.");
            }

            // Verificar documento único (excluyendo el trabajador actual)
            var workers = await _unitOfWork.GetRepository<Worker>().GetAllAsync();
            if (workers.Any(w => w.IdentityDocument == dto.IdentityDocument && w.WorkerId != dto.WorkerId))
            {
                throw new ValidationException("IdentityDocument", $"Ya existe un trabajador con el documento '{dto.IdentityDocument}'");
            }

            // Verificar email único (excluyendo el trabajador actual)
            if (workers.Any(w => w.Email == dto.Email && w.WorkerId != dto.WorkerId))
            {
                throw new ValidationException("Email", $"Ya existe un trabajador con el correo electrónico '{dto.Email}'");
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