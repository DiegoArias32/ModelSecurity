using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Data;
using Entity.Model;
using Entity.DTOs;

namespace Business
{
    public class RolUserBusiness : GenericBusiness<RolUserDto, RolUser>
    {
        private readonly RolUserData _rolUserData;

        public RolUserBusiness(RolUserData rolUserData, ILogger<RolUserBusiness> logger)
            : base(rolUserData, logger)
        {
            _rolUserData = rolUserData;
        }

        // Implementación de los métodos abstractos requeridos
        protected override void ValidateDto(RolUserDto dto)
        {
            if (dto == null)
                throw new ArgumentException("El objeto RolUserDto no puede ser nulo");
                
            if (dto.UserId <= 0)
                throw new ArgumentException("El UserId debe ser mayor que cero");
                
            if (dto.RolId <= 0)
                throw new ArgumentException("El RolId debe ser mayor que cero");
        }

        protected override RolUser MapToEntity(RolUserDto dto)
        {
            return new RolUser
            {
                Id = dto.Id,
                UserId = dto.UserId,
                RolId = dto.RolId,
            };
        }

        protected override RolUserDto MapToDto(RolUser entity)
        {
            return new RolUserDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                RolId = entity.RolId,
            };
        }

        protected override IEnumerable<RolUserDto> MapToDtoList(IEnumerable<RolUser> entities)
        {
            return entities.Select(MapToDto).ToList();
        }
    }
}