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
        private readonly ILogger _logger;

        public PermissionBusiness(PermissionData permissionData, ILogger<PermissionBusiness> logger)
        {
            _permissionData = permissionData ?? throw new ArgumentNullException(nameof(permissionData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PermissionDto> CreateAsync(PermissionDto permissionDto)
        {
            var permission = new Permission
            {
                Can_Read = permissionDto.CanRead,
                Can_Create = permissionDto.CanCreate,
                Can_Update = permissionDto.CanUpdate,
                Can_Delete = permissionDto.CanDelete,
                CreateAt = DateTime.UtcNow
            };

            var createdPermission = await _permissionData.CreateAsync(permission);

            return new PermissionDto
            {
                Id = createdPermission.Id,
                CanRead = createdPermission.Can_Read,
                CanCreate = createdPermission.Can_Create,
                CanUpdate = createdPermission.Can_Update,
                CanDelete = createdPermission.Can_Delete
            };
        }

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var permissions = await _permissionData.GetAllAsync();

            var permissionDtos = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                permissionDtos.Add(new PermissionDto
                {
                    Id = permission.Id,
                    CanRead = permission.Can_Read,
                    CanCreate = permission.Can_Create,
                    CanUpdate = permission.Can_Update,
                    CanDelete = permission.Can_Delete
                });
            }

            return permissionDtos;
        }

        public async Task<PermissionDto?> GetByIdAsync(int id)
        {
            var permission = await _permissionData.GetByIdAsync(id);
            if (permission == null)
                return null;

            return new PermissionDto
            {
                Id = permission.Id,
                CanRead = permission.Can_Read,
                CanCreate = permission.Can_Create,
                CanUpdate = permission.Can_Update,
                CanDelete = permission.Can_Delete
            };
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
    }
}
