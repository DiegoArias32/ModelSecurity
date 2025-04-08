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
    public class RolUserBusiness
    {
        private readonly RolUserData _rolUserData;
        private readonly ILogger _logger;

        public RolUserBusiness(RolUserData rolUserData, ILogger logger)
        {
            _rolUserData = rolUserData ?? throw new ArgumentNullException(nameof(rolUserData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RolUserDto> CreateAsync(RolUserDto rolUserDto)
        {
            try
            {
                if (rolUserDto.UserId <= 0 || rolUserDto.RolId <= 0)
                {
                    _logger.LogWarning("Usuario o Rol no son válidos.");
                    throw new ArgumentException("El usuario o rol no son válidos.");
                }

                var rolUser = MapToEntity(rolUserDto);
                rolUser.CreateAt = DateTime.UtcNow;
                rolUser.DeleteAt = DateTime.MinValue;

                var createdRolUser = await _rolUserData.CreateAsync(rolUser);
                return MapToDTO(createdRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la relación Rol-User.");
                throw;
            }
        }

        public async Task<IEnumerable<RolUserDto>> GetAllAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                return MapToDTOList(rolUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las relaciones Rol-User.");
                throw;
            }
        }

        public async Task<RolUserDto?> GetByIdAsync(int id)
        {
            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                return rolUser != null ? MapToDTO(rolUser) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación Rol-User con ID {RolUserId}.", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(RolUserDto rolUserDto)
        {
            try
            {
                if (rolUserDto.UserId <= 0 || rolUserDto.RolId <= 0)
                {
                    _logger.LogWarning("Usuario o Rol no son válidos.");
                    throw new ArgumentException("El usuario o rol no son válidos.");
                }

                var rolUser = MapToEntity(rolUserDto);
                return await _rolUserData.UpdateAsync(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación Rol-User.");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _rolUserData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la relación Rol-User con ID {RolUserId}.", id);
                return false;
            }
        }

        // -----------------------
        // MÉTODOS DE MAPEADO
        // -----------------------

        private RolUserDto MapToDTO(RolUser entity)
        {
            return new RolUserDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                RolId = entity.RolId,
                CreateAt = entity.CreateAt,
                DeleteAt = entity.DeleteAt
            };
        }

        private RolUser MapToEntity(RolUserDto dto)
        {
            return new RolUser
            {
                Id = dto.Id,
                UserId = dto.UserId,
                RolId = dto.RolId,
                CreateAt = dto.CreateAt,
                DeleteAt = dto.DeleteAt
            };
        }

        private IEnumerable<RolUserDto> MapToDTOList(IEnumerable<RolUser> list)
        {
            return list.Select(MapToDTO).ToList();
        }
    }
}
