using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class PermissionBusiness : GenericBusiness<PermissionDto, Permission>
    {
        public PermissionBusiness(IGenericData<Permission> data, ILogger<PermissionBusiness> logger)
            : base(data, logger)
        {
        }
        
        // Sobrescribimos UpdateAsync para manejar la actualización específica
        // porque la implementación original no utiliza MapToEntity directamente
        public override async Task<bool> UpdateAsync(PermissionDto permissionDto)
        {
            try
            {
                var permission = await _data.GetByIdAsync(permissionDto.Id);
                if (permission == null)
                    return false;

                // Actualización manual de los campos
                permission.Can_Read = permissionDto.CanRead;
                permission.Can_Create = permissionDto.CanCreate;
                permission.Can_Update = permissionDto.CanUpdate;
                permission.Can_Delete = permissionDto.CanDelete;

                return await _data.UpdateAsync(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso con ID {PermissionId}", permissionDto.Id);
                return false;
            }
        }

        // Implementación de los métodos abstractos requeridos
        protected override void ValidateDto(PermissionDto dto)
        {
            // No hay validaciones específicas en la implementación original,
            // pero podríamos añadir algunas si fuera necesario
            if (dto == null)
                throw new ArgumentException("El objeto permiso no puede ser nulo");
        }

        protected override Permission MapToEntity(PermissionDto dto)
        {
            return new Permission
            {
                Id = dto.Id,
                Can_Read = dto.CanRead,
                Can_Create = dto.CanCreate,
                Can_Update = dto.CanUpdate,
                Can_Delete = dto.CanDelete
            };
        }

        protected override PermissionDto MapToDto(Permission entity)
        {
            return new PermissionDto
            {
                Id = entity.Id,
                CanRead = entity.Can_Read,
                CanCreate = entity.Can_Create,
                CanUpdate = entity.Can_Update,
                CanDelete = entity.Can_Delete
            };
        }

        protected override IEnumerable<PermissionDto> MapToDtoList(IEnumerable<Permission> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}