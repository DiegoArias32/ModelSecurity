using System;
using AutoMapper;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Interfaces;
using Utilities.Services;

namespace Utilities.Services
{
    public class RolService : Service<Rol, RolDto>
    {
        private readonly IValidationStrategy<RolDto> _validationStrategy;

        public RolService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<Rol, RolDto>> logger,
            IValidationStrategy<RolDto> validationStrategy = null)  // Hacerlo opcional
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<RolDto> CreateAsync(RolDto dto)
        {
            // Validar usando la estrategia solo si está disponible
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(RolDto dto)
        {
            // Validar usando la estrategia solo si está disponible
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.UpdateAsync(dto);
        }
    }
}