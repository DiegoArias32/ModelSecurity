using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class RolFormPermissionBusiness
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly ILogger _logger;

        public RolFormPermissionBusiness(RolFormPermissionData rolFormPermissionData, ILogger logger)
        {
            _rolFormPermissionData = rolFormPermissionData ?? throw new ArgumentNullException(nameof(rolFormPermissionData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo permiso de formulario para un rol.
        /// </summary>
        public async Task<RolFormPermissionDto> CreateRolFormPermissionAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                // Validación
                if (rolFormPermissionDto == null)
                {
                    throw new ValidationException("El objeto RolFormPermissionDto no puede ser nulo");
                }

                var rolFormPermission = new RolFormPermission
                {
                    RolId = rolFormPermissionDto.RolId,
                    FormId = rolFormPermissionDto.FormId,
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = DateTime.MinValue // Asumimos que la eliminación es indefinida al principio
                };

                var createdRolFormPermission = await _rolFormPermissionData.CreateAsync(rolFormPermission);

                return new RolFormPermissionDto
                {
                    Id = createdRolFormPermission.Id,
                    RolId = createdRolFormPermission.RolId,
                    FormId = createdRolFormPermission.FormId,
                    CreateAt = createdRolFormPermission.CreateAt,
                    DeleteAt = createdRolFormPermission.DeleteAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso de formulario para el rol");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso de formulario para el rol", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los permisos de formulario para roles.
        /// </summary>
        public async Task<IEnumerable<RolFormPermissionDto>> GetAllRolFormPermissionsAsync()
        {
            try
            {
                var rolFormPermissions = await _rolFormPermissionData.GetAllAsync();

                var rolFormPermissionsDto = new List<RolFormPermissionDto>();

                foreach (var rolFormPermission in rolFormPermissions)
                {
                    rolFormPermissionsDto.Add(new RolFormPermissionDto
                    {
                        Id = rolFormPermission.Id,
                        RolId = rolFormPermission.RolId,
                        FormId = rolFormPermission.FormId,
                        CreateAt = rolFormPermission.CreateAt,
                        DeleteAt = rolFormPermission.DeleteAt
                    });
                }

                return rolFormPermissionsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de formularios para roles");
                throw new ExternalServiceException("Base de datos", "Error al obtener los permisos de formularios para roles", ex);
            }
        }

        /// <summary>
        /// Obtiene un permiso de formulario para rol por su identificador.
        /// </summary>
        public async Task<RolFormPermissionDto> GetRolFormPermissionByIdAsync(int id)
        {
            try
            {
                var rolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);

                if (rolFormPermission == null)
                {
                    throw new EntityNotFoundException("RolFormPermission", id);
                }

                return new RolFormPermissionDto
                {
                    Id = rolFormPermission.Id,
                    RolId = rolFormPermission.RolId,
                    FormId = rolFormPermission.FormId,
                    CreateAt = rolFormPermission.CreateAt,
                    DeleteAt = rolFormPermission.DeleteAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso de formulario para el rol con ID {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al obtener el permiso de formulario para el rol", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los permisos de formulario para un rol específico.
        /// </summary>
        public async Task<IEnumerable<RolFormPermissionDto>> GetRolFormPermissionsByRolIdAsync(int rolId)
        {
            try
            {
                var rolFormPermissions = await _rolFormPermissionData.GetByRolIdAsync(rolId);

                var rolFormPermissionsDto = new List<RolFormPermissionDto>();

                foreach (var rolFormPermission in rolFormPermissions)
                {
                    rolFormPermissionsDto.Add(new RolFormPermissionDto
                    {
                        Id = rolFormPermission.Id,
                        RolId = rolFormPermission.RolId,
                        FormId = rolFormPermission.FormId,
                        CreateAt = rolFormPermission.CreateAt,
                        DeleteAt = rolFormPermission.DeleteAt
                    });
                }

                return rolFormPermissionsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos de formularios para el rol con ID {RolId}", rolId);
                throw new ExternalServiceException("Base de datos", "Error al obtener los permisos de formularios para el rol", ex);
            }
        }

        /// <summary>
        /// Actualiza un permiso de formulario para rol existente.
        /// </summary>
        public async Task<bool> UpdateRolFormPermissionAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                var rolFormPermission = new RolFormPermission
                {
                    Id = rolFormPermissionDto.Id,
                    RolId = rolFormPermissionDto.RolId,
                    FormId = rolFormPermissionDto.FormId,
                    CreateAt = rolFormPermissionDto.CreateAt,
                    DeleteAt = rolFormPermissionDto.DeleteAt
                };

                return await _rolFormPermissionData.UpdateAsync(rolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso de formulario para el rol");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el permiso de formulario para el rol", ex);
            }
        }

        /// <summary>
        /// Elimina un permiso de formulario para rol.
        /// </summary>
        public async Task<bool> DeleteRolFormPermissionAsync(int id)
        {
            try
            {
                return await _rolFormPermissionData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso de formulario para el rol con ID {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el permiso de formulario para el rol", ex);
            }
        }

        /// <summary>
        /// Elimina permanentemente un permiso de formulario para rol.
        /// </summary>
        public async Task<bool> PermanentDeleteRolFormPermissionAsync(int id)
        {
            try
            {
                return await _rolFormPermissionData.PermanentDeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el permiso de formulario para el rol con ID {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el permiso de formulario para el rol", ex);
            }
        }
    }
}
