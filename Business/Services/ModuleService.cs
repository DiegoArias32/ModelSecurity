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
using Module = Entity.Model.Module;

namespace Business.Services
{
    public class ModuleService : Service<Module, ModuleDto>
    {
        private readonly IValidationStrategy<ModuleDto> _validationStrategy;

        public ModuleService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<Module, ModuleDto>> logger,
            IValidationStrategy<ModuleDto> validationStrategy = null) 
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<ModuleDto> CreateAsync(ModuleDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new ValidationException("Code", "El código del módulo es obligatorio.");
            }

            // Verificar código único
            var modules = await _unitOfWork.GetRepository<Module>().GetAllAsync();
            if (modules.Any(m => m.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Code", $"Ya existe un módulo con el código '{dto.Code}'");
            }

            // Aplicar estrategia de validación adicional si existe
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            // Asignar valores predeterminados
            dto.Active = true; // Por defecto, los módulos se crean activos

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(ModuleDto dto)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new ValidationException("Code", "El código del módulo es obligatorio.");
            }

            // Verificar código único (excluyendo el módulo actual)
            var modules = await _unitOfWork.GetRepository<Module>().GetAllAsync();
            if (modules.Any(m => m.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase) && m.Id != dto.Id))
            {
                throw new ValidationException("Code", $"Ya existe un módulo con el código '{dto.Code}'");
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