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
    public class FormService : Service<Form, FormDto>
    {
        private readonly IValidationStrategy<FormDto> _validationStrategy;

        public FormService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<Form, FormDto>> logger,
            IValidationStrategy<FormDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<FormDto> CreateAsync(FormDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ValidationException("Name", "El nombre del formulario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new ValidationException("Code", "El código del formulario es obligatorio.");
            }

            // Verificar código único
            var forms = await _unitOfWork.GetRepository<Form>().GetAllAsync();
            if (forms.Any(f => f.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Code", $"Ya existe un formulario con el código '{dto.Code}'");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            // Asignar valores predeterminados
            dto.Active = true; // Por defecto, los formularios se crean activos

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(FormDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ValidationException("Name", "El nombre del formulario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new ValidationException("Code", "El código del formulario es obligatorio.");
            }

            // Verificar código único (excluyendo el formulario actual)
            var forms = await _unitOfWork.GetRepository<Form>().GetAllAsync();
            if (forms.Any(f => f.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase) && f.Id != dto.Id))
            {
                throw new ValidationException("Code", $"Ya existe un formulario con el código '{dto.Code}'");
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