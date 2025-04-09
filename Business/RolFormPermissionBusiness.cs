using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
public class RolFormPermissionBusiness
{
    private readonly RolFormPermissionData _rolFormPermissionData;
    private readonly ILogger<RolFormPermissionBusiness> _logger;

    public RolFormPermissionBusiness(RolFormPermissionData rolFormPermissionData, ILogger<RolFormPermissionBusiness> logger)
    {
        _rolFormPermissionData = rolFormPermissionData ?? throw new ArgumentNullException(nameof(rolFormPermissionData));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

        public async Task<RolFormPermissionDto> CreateRolFormPermissionAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                if (rolFormPermissionDto == null)
                {
                    throw new ValidationException("El objeto RolFormPermissionDto no puede ser nulo");
                }

                var rolFormPermission = MapToEntity(rolFormPermissionDto);
                rolFormPermission.CreateAt = DateTime.UtcNow;
                rolFormPermission.DeleteAt = DateTime.MinValue;

                var created = await _rolFormPermissionData.CreateAsync(rolFormPermission);
                return MapToDTO(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso de formulario para el rol");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso de formulario para el rol", ex);
            }
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetAllRolFormPermissionsAsync()
        {
            try
            {
                var list = await _rolFormPermissionData.GetAllAsync();
                return MapToDTOList(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de formularios para roles");
                throw new ExternalServiceException("Base de datos", "Error al obtener los permisos de formularios para roles", ex);
            }
        }

        public async Task<RolFormPermissionDto> GetRolFormPermissionByIdAsync(int id)
        {
            try
            {
                var item = await _rolFormPermissionData.GetByIdAsync(id);
                if (item == null)
                {
                    throw new EntityNotFoundException("RolFormPermission", id);
                }

                return MapToDTO(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso de formulario para el rol con ID {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al obtener el permiso de formulario para el rol", ex);
            }
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetRolFormPermissionsByRolIdAsync(int rolId)
        {
            try
            {
                var list = await _rolFormPermissionData.GetByRolIdAsync(rolId);
                return MapToDTOList(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos de formularios para el rol con ID {RolId}", rolId);
                throw new ExternalServiceException("Base de datos", "Error al obtener los permisos de formularios para el rol", ex);
            }
        }

        public async Task<bool> UpdateRolFormPermissionAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                var entity = MapToEntity(rolFormPermissionDto);
                return await _rolFormPermissionData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso de formulario para el rol");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el permiso de formulario para el rol", ex);
            }
        }

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

        // -----------------------
        // MÉTODOS DE MAPEADO
        // -----------------------

        private RolFormPermissionDto MapToDTO(RolFormPermission rolFormPermission)
        {
            return new RolFormPermissionDto
            {
                Id = rolFormPermission.Id,
                RolId = rolFormPermission.RolId,
                FormId = rolFormPermission.FormId,
                PermissionId = rolFormPermission.PermissionId
            };
        }

        private RolFormPermission MapToEntity(RolFormPermissionDto rolFormPermissionDto)
        {
            return new RolFormPermission
            {
                Id = rolFormPermissionDto.Id,
                RolId = rolFormPermissionDto.RolId,
                FormId = rolFormPermissionDto.FormId,
                PermissionId = rolFormPermissionDto.PermissionId,
            };
        }

        private IEnumerable<RolFormPermissionDto> MapToDTOList(IEnumerable<RolFormPermission> list)
        {
            return list.Select(MapToDTO).ToList();
        }
    }
}
