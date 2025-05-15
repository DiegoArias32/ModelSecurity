using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class RolFormPermissionBusiness : GenericBusiness<RolFormPermissionDto, RolFormPermission>
    {
        private readonly RolFormPermissionData _rolFormPermissionData;

        public RolFormPermissionBusiness(RolFormPermissionData rolFormPermissionData, ILogger<RolFormPermissionBusiness> logger)
            : base(rolFormPermissionData, logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetRolFormPermissionsByRolIdAsync(int rolId)
        {
            var permissions = await _rolFormPermissionData.GetByRolIdAsync(rolId);
            return MapToDtoList(permissions);
        }

        protected override void ValidateDto(RolFormPermissionDto dto)
        {
            if (dto == null) throw new ArgumentException("El objeto RolFormPermissionDto no puede ser nulo");
            if (dto.RolId <= 0) throw new ArgumentException("El RolId debe ser mayor que cero");
            if (dto.FormId <= 0) throw new ArgumentException("El FormId debe ser mayor que cero");
            if (dto.PermissionId <= 0) throw new ArgumentException("El PermissionId debe ser mayor que cero");
        }

        protected override RolFormPermission MapToEntity(RolFormPermissionDto dto)
        {
            return new RolFormPermission
            {
                Id = dto.Id,
                RolId = dto.RolId,
                FormId = dto.FormId,
                PermissionId = dto.PermissionId,
                CreateAt = dto.Id == 0 ? DateTime.UtcNow : DateTime.UtcNow, // Nueva entidad o actualización
                DeleteAt = null
            };
        }

        protected override RolFormPermissionDto MapToDto(RolFormPermission entity)
        {
            return new RolFormPermissionDto
            {
                Id = entity.Id,
                RolId = entity.RolId,
                FormId = entity.FormId,
                PermissionId = entity.PermissionId
            };
        }

        protected override IEnumerable<RolFormPermissionDto> MapToDtoList(IEnumerable<RolFormPermission> entities)
        {
            return entities.Select(MapToDto).ToList();
        }
    }
}