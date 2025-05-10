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
    public class UserService : Service<User, UserDto>
    {
        private readonly IValidationStrategy<UserDto> _validationStrategy;

        public UserService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<Service<User, UserDto>> logger,
            IValidationStrategy<UserDto> validationStrategy = null)  // Hacerlo opcional
            : base(unitOfWork, mapper, logger)
        {
            _validationStrategy = validationStrategy;
        }

        public override async Task<UserDto> CreateAsync(UserDto dto)
        {
            // Validar usando la estrategia solo si está disponible
            if (_validationStrategy != null && !_validationStrategy.IsValid(dto))
            {
                throw new ValidationException(_validationStrategy.GetErrorMessage());
            }

            return await base.CreateAsync(dto);
        }

        public override async Task<bool> UpdateAsync(UserDto dto)
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