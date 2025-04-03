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

        /// <summary>
        /// Crea un nuevo permiso.
        /// </summary>
        /// <param name="permissionDto">Objeto DTO con los datos del permiso a crear.</param>
        /// <returns>El permiso creado.</returns>
        public async Task<PermissionDto> CreateAsync(PermissionDto permissionDto)
        {
            // Convertimos el DTO a entidad
            var permission = new Permission
            {
                Can_Read = permissionDto.CanRead ? "Yes" : "No",
                Can_Create = permissionDto.CanCreate ? "Yes" : "No",
                Can_Update = permissionDto.CanUpdate ? "Yes" : "No",
                Can_Delete = permissionDto.CanDelete ? "Yes" : "No",
                CreateAt = DateTime.UtcNow
            };

            // Llamamos al método Data para guardar en la base de datos
            var createdPermission = await _permissionData.CreateAsync(permission);

            // Convertimos la entidad a DTO y la retornamos
            return new PermissionDto
            {
                Id = createdPermission.Id,
                CanRead = createdPermission.Can_Read == "Yes",
                CanCreate = createdPermission.Can_Create == "Yes",
                CanUpdate = createdPermission.Can_Update == "Yes",
                CanDelete = createdPermission.Can_Delete == "Yes"
            };
        }

        /// <summary>
        /// Obtiene todos los permisos.
        /// </summary>
        /// <returns>Lista de permisos.</returns>
        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var permissions = await _permissionData.GetAllAsync();

            // Convertimos cada entidad Permission a su correspondiente DTO
            var permissionDtos = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                permissionDtos.Add(new PermissionDto
                {
                    Id = permission.Id,
                    CanRead = permission.Can_Read == "Yes",
                    CanCreate = permission.Can_Create == "Yes",
                    CanUpdate = permission.Can_Update == "Yes",
                    CanDelete = permission.Can_Delete == "Yes"
                });
            }

            return permissionDtos;
        }

        /// <summary>
        /// Obtiene un permiso por su ID.
        /// </summary>
        /// <param name="id">ID del permiso.</param>
        /// <returns>El permiso correspondiente al ID proporcionado.</returns>
        public async Task<PermissionDto?> GetByIdAsync(int id)
        {
            var permission = await _permissionData.GetByIdAsync(id);
            if (permission == null)
                return null;

            // Convertimos la entidad Permission a su DTO
            return new PermissionDto
            {
                Id = permission.Id,
                CanRead = permission.Can_Read == "Yes",
                CanCreate = permission.Can_Create == "Yes",
                CanUpdate = permission.Can_Update == "Yes",
                CanDelete = permission.Can_Delete == "Yes"
            };
        }

        /// <summary>
        /// Actualiza un permiso existente.
        /// </summary>
        /// <param name="permissionDto">DTO con los datos del permiso a actualizar.</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario.</returns>
        public async Task<bool> UpdateAsync(PermissionDto permissionDto)
        {
            var permission = await _permissionData.GetByIdAsync(permissionDto.Id);
            if (permission == null)
                return false;

            // Actualizamos los campos del permiso
            permission.Can_Read = permissionDto.CanRead ? "Yes" : "No";
            permission.Can_Create = permissionDto.CanCreate ? "Yes" : "No";
            permission.Can_Update = permissionDto.CanUpdate ? "Yes" : "No";
            permission.Can_Delete = permissionDto.CanDelete ? "Yes" : "No";

            // Llamamos al Data para actualizar
            return await _permissionData.UpdateAsync(permission);
        }

        /// <summary>
        /// Elimina un permiso por su ID.
        /// </summary>
        /// <param name="id">ID del permiso a eliminar.</param>
        /// <returns>True si se eliminó correctamente, false en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            return await _permissionData.DeleteAsync(id);
        }
    }
}
