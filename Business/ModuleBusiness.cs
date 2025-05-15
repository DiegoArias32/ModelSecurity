using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class ModuleBusiness : GenericBusiness<ModuleDto, Module>
    {
        public ModuleBusiness(IGenericData<Module> data, ILogger<ModuleBusiness> logger)
            : base(data, logger)
        {
        }

        // Implementación de los métodos abstractos requeridos por GenericBusiness
        
        protected override void ValidateDto(ModuleDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El objeto módulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                _logger.LogWarning("Se intentó crear un módulo con Code vacío");
                throw new ValidationException("Code", "El Code del módulo es obligatorio");
            }
        }

        protected override Module MapToEntity(ModuleDto dto)
        {
            return new Module
            {
                Id = dto.Id,
                Code = dto.Code,
                Active = dto.Active,
            };
        }

        protected override ModuleDto MapToDto(Module entity)
        {
            return new ModuleDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Active = entity.Active,
            };
        }

        protected override IEnumerable<ModuleDto> MapToDtoList(IEnumerable<Module> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}