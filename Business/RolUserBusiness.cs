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

        /// <summary>
        /// Constructor que recibe la instancia de RolUserData y el logger.
        /// </summary>
        /// <param name="rolUserData">Instancia de <see cref="RolUserData"/> para acceder a la capa de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
        public RolUserBusiness(RolUserData rolUserData, ILogger logger)
        {
            _rolUserData = rolUserData ?? throw new ArgumentNullException(nameof(rolUserData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea una nueva relación Rol-User tras realizar validaciones de negocio.
        /// </summary>
        /// <param name="rolUser">Instancia de <see cref="RolUserDto"/> a crear.</param>
        /// <returns>La instancia del Rol-User creado.</returns>
        public async Task<RolUserDto> CreateAsync(RolUserDto rolUserDto)
        {
            try
            {
                // Validaciones de negocio antes de crear
                if (rolUserDto.UserId <= 0 || rolUserDto.RolId <= 0)
                {
                    _logger.LogWarning("Usuario o Rol no son válidos.");
                    throw new ArgumentException("El usuario o rol no son válidos.");
                }

                // Mapeo DTO -> Entidad
                var rolUser = new RolUser
                {
                    UserId = rolUserDto.UserId,
                    RolId = rolUserDto.RolId,
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = DateTime.MinValue  // Se podría establecer un valor predeterminado
                };

                // Llamar al método de la capa de datos para crear el Rol-User
                var createdRolUser = await _rolUserData.CreateAsync(rolUser);

                // Mapeo de la entidad creada a DTO
                return _rolUserData.ToDto(createdRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la relación Rol-User.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las relaciones Rol-User.
        /// </summary>
        /// <returns>Lista de DTOs RolUserDto.</returns>
        public async Task<IEnumerable<RolUserDto>> GetAllAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                return _rolUserData.ToDtoList(rolUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las relaciones Rol-User.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una relación Rol-User específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la relación Rol-User.</param>
        /// <returns>DTO RolUserDto encontrado o null si no existe.</returns>
        public async Task<RolUserDto?> GetByIdAsync(int id)
        {
            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                return rolUser != null ? _rolUserData.ToDto(rolUser) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación Rol-User con ID {RolUserId}.", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza una relación Rol-User existente.
        /// </summary>
        /// <param name="rolUserDto">El DTO con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False si no.</returns>
        public async Task<bool> UpdateAsync(RolUserDto rolUserDto)
        {
            try
            {
                // Validaciones de negocio antes de actualizar
                if (rolUserDto.UserId <= 0 || rolUserDto.RolId <= 0)
                {
                    _logger.LogWarning("Usuario o Rol no son válidos.");
                    throw new ArgumentException("El usuario o rol no son válidos.");
                }

                // Mapeo DTO -> Entidad
                var rolUser = new RolUser
                {
                    Id = rolUserDto.Id,
                    UserId = rolUserDto.UserId,
                    RolId = rolUserDto.RolId,
                    CreateAt = rolUserDto.CreateAt,
                    DeleteAt = rolUserDto.DeleteAt
                };

                return await _rolUserData.UpdateAsync(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación Rol-User.");
                return false;
            }
        }

        /// <summary>
        /// Elimina una relación Rol-User.
        /// </summary>
        /// <param name="id">Identificador de la relación Rol-User a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
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
    }
}
