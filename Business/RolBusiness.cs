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
    public class RolBusiness : GenericBusiness<RolDto, Rol>
    {
        private readonly RolData _rolData;

        public RolBusiness(RolData rolData, ILogger<RolBusiness> logger)
            : base(rolData, logger)
        {
            _rolData = rolData;
        }

        // Implementación de los métodos abstractos requeridos
        protected override void ValidateDto(RolDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new ValidationException("Name", "El Name del rol es obligatorio");
            }
        }

        protected override Rol MapToEntity(RolDto dto)
        {
            return new Rol
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }

        protected override RolDto MapToDto(Rol entity)
        {
            return new RolDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };
        }

        protected override IEnumerable<RolDto> MapToDtoList(IEnumerable<Rol> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}