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
    public class FormModuleService : Service<FormModule, FormModuleDto>
    {
        private readonly IValidationStrategy<FormModuleDto> _validationStrategy;

        public FormModuleService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<FormModule, FormModuleDto>> logger,
            IValidationStrategy<FormModuleDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<FormModuleDto> CreateAsync(FormModuleDto dto)
        {
            // Validación básica
            if (dto.ModuleId <= 0)
            {
                throw new ValidationException("ModuleId", "El ID del módulo debe ser mayor que cero.");
            }

            if (dto.FormId <= 0)
            {
                throw new ValidationException("FormId", "El ID del formulario debe ser mayor que cero.");
            }

            // Verificar que el módulo existe
            var moduleRepo = _unitOfWork.GetRepository<Module>();
            var module = await moduleRepo.GetByIdAsync(dto.ModuleId);
            if (module == null)
            {
                throw new ValidationException("ModuleId", $"No existe un módulo con ID {dto.ModuleId}");
            }

            // Verificar que el formulario existe
            var formRepo = _unitOfWork.GetRepository<Form>();
            var form = await formRepo.GetByIdAsync(dto.FormId);
            if (form == null)
            {
                throw new ValidationException("FormId", $"No existe un formulario con ID {dto.FormId}");
            }

            // Verificar que la asignación no existe ya
            var formModules = await _unitOfWork.GetRepository<FormModule>().GetAllAsync();
            if (formModules.Any(fm => fm.ModuleId == dto.ModuleId && fm.FormId == dto.FormId))
            {
                throw new ValidationException("FormModule", $"El formulario con ID {dto.FormId} ya está asignado al módulo con ID {dto.ModuleId}");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(FormModuleDto dto)
        {
            // Validación básica
            if (dto.ModuleId <= 0)
            {
                throw new ValidationException("ModuleId", "El ID del módulo debe ser mayor que cero.");
            }

            if (dto.FormId <= 0)
            {
                throw new ValidationException("FormId", "El ID del formulario debe ser mayor que cero.");
            }

            // Verificar que el módulo existe
            var moduleRepo = _unitOfWork.GetRepository<Module>();
            var module = await moduleRepo.GetByIdAsync(dto.ModuleId);
            if (module == null)
            {
                throw new ValidationException("ModuleId", $"No existe un módulo con ID {dto.ModuleId}");
            }

            // Verificar que el formulario existe
            var formRepo = _unitOfWork.GetRepository<Form>();
            var form = await formRepo.GetByIdAsync(dto.FormId);
            if (form == null)
            {
                throw new ValidationException("FormId", $"No existe un formulario con ID {dto.FormId}");
            }

            // Verificar que la asignación no existe ya (excluyendo la actual)
            var formModules = await _unitOfWork.GetRepository<FormModule>().GetAllAsync();
            if (formModules.Any(fm => fm.ModuleId == dto.ModuleId && fm.FormId == dto.FormId && fm.Id != dto.Id))
            {
                throw new ValidationException("FormModule", $"El formulario con ID {dto.FormId} ya está asignado al módulo con ID {dto.ModuleId}");
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