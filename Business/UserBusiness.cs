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
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<UserBusiness> _logger;

        /// <summary>
        /// Constructor que recibe la instancia de UserData y el logger.
        /// </summary>
        /// <param name="userData">Instancia de <see cref="UserData"/> para acceder a la capa de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
        {
            _userData = userData ?? throw new ArgumentNullException(nameof(userData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo usuario tras realizar validaciones de negocio.
        /// </summary>
        /// <param name="userDto">Instancia de <see cref="UserDto"/> a crear.</param>
        /// <returns>La instancia del usuario creado.</returns>
        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            try
            {
                // Validaciones de negocio antes de crear
                if (string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Email))
                {
                    _logger.LogWarning("Name o Email no pueden estar vacíos.");
                    throw new ArgumentException("Name o Email no pueden estar vacíos.");
                }

                // Mapeo DTO -> Entidad
                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Password = "", // Debería de manejarse un hash de la contraseña en la lógica real.
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = DateTime.MinValue // Se podría establecer un valor predeterminado.
                };

                // Llamar al método de la capa de datos para crear el usuario
                var createdUser = await _userData.CreateAsync(user);

                // Mapeo de la entidad creada a DTO
                return new UserDto
                {
                    Id = createdUser.Id,
                    Name = createdUser.Name,
                    Email = createdUser.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        /// <returns>Lista de DTOs UserDto.</returns>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                return users.Select(user => new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>El DTO UserDto encontrado o null si no existe.</returns>
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user != null)
                {
                    return new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {UserId}.", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="userDto">DTO con la información actualizada del usuario.</param>
        /// <returns>True si la operación fue exitosa, False si no.</returns>
        public async Task<bool> UpdateAsync(UserDto userDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Email))
                {
                    _logger.LogWarning("Name o Email no pueden estar vacíos.");
                    throw new ArgumentException("Name o Email no pueden estar vacíos.");
                }

                // Buscar el usuario existente
                var user = await _userData.GetByIdAsync(userDto.Id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado para actualizar.");
                    return false;
                }

                // Mapeo DTO -> Entidad para actualizar
                user.Name = userDto.Name;
                user.Email = userDto.Email;
                user.Password = user.Password; // Asegurarse de manejar la contraseña correctamente

                var updated = await _userData.UpdateAsync(user);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID {UserId}.", userDto.Id);
                return false;
            }
        }

        /// <summary>
        /// Elimina un usuario.
        /// </summary>
        /// <param name="id">Identificador del usuario a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False si no.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado para eliminar.");
                    return false;
                }

                return await _userData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID {UserId}.", id);
                return false;
            }
        }
    }
}
