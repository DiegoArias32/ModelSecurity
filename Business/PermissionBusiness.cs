using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger<PermissionBusiness> _logger;

        public PermissionBusiness(PermissionData permissionData, ILogger<PermissionBusiness> logger)
        {
            _permissionData = permissionData ?? throw new ArgumentNullException(nameof(permissionData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PermissionDto> CreateAsync(PermissionDto permissionDto)
        {
            var permission = MapToEntity(permissionDto);
            permission.CreateAt = DateTime.UtcNow;

            var createdPermission = await _permissionData.CreateAsync(permission);
            return MapToDTO(createdPermission);
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var permission = await _permissionData.GetByIdAsync(id);
        if (permission == null)
            return false;

        return await _permissionData.PermanentDeleteAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el permiso.");
        return false;
    }
}

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var permissions = await _permissionData.GetAllAsync();
            return MapToDTOList(permissions);
        }

        public async Task<PermissionDto?> GetByIdAsync(int id)
        {
            var permission = await _permissionData.GetByIdAsync(id);
            if (permission == null)
                return null;

            return MapToDTO(permission);
        }

        public async Task<bool> UpdateAsync(PermissionDto permissionDto)
        {
            var permission = await _permissionData.GetByIdAsync(permissionDto.Id);
            if (permission == null)
                return false;

            permission.Can_Read = permissionDto.CanRead;
            permission.Can_Create = permissionDto.CanCreate;
            permission.Can_Update = permissionDto.CanUpdate;
            permission.Can_Delete = permissionDto.CanDelete;

            return await _permissionData.UpdateAsync(permission);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _permissionData.DeleteAsync(id);
        }

        // -----------------------
        // MÉTODOS DE MAPEADO
        // -----------------------

        private PermissionDto MapToDTO(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                CanRead = permission.Can_Read,
                CanCreate = permission.Can_Create,
                CanUpdate = permission.Can_Update,
                CanDelete = permission.Can_Delete
            };
        }

        private Permission MapToEntity(PermissionDto dto)
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

        private IEnumerable<PermissionDto> MapToDTOList(IEnumerable<Permission> permissions)
        {
            return permissions.Select(MapToDTO).ToList();
        }

    }
}
