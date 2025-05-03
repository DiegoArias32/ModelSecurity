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
                var user = MapToEntity(userDto);

                // Llamar al método de la capa de datos para crear el usuario
                var createdUser = await _userData.CreateAsync(user);

                // Mapeo de la entidad creada a DTO
                return MapToDTO(createdUser);
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
                return MapToDTOList(users); // Usamos el método MapToDTOList aquí
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
                    return MapToDTO(user); // Usamos el método MapToDTO aquí
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {UserId}.", id);
                throw;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        if (id <= 0)
        {
            _logger.LogWarning("ID de usuario inválido para eliminación permanente: {UserId}", id);
            throw new ArgumentException("El ID debe ser mayor que cero");
        }
        
        var user = await _userData.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Usuario no encontrado para eliminación permanente.");
            return false;
        }

        return await _userData.PermanentDeleteAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el usuario con ID {UserId}.", id);
        return false;
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
                MapToEntity(userDto, user);

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

        // Método para mapear de User a UserDto
        private UserDto MapToDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
            };
        }

        // Método para mapear de UserDto a User
        private User MapToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password, // Aquí debería implementarse el hash de la contraseña
            };
        }

        // Método para mapear de UserDto a User (cuando ya tenemos un usuario)
        private void MapToEntity(UserDto userDto, User user)
        {
            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.Password = userDto.Password; // Asegurarse de manejar la contraseña correctamente
        }

        // Método para mapear una lista de User a una lista de UserDto
        private IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
        {
            return users.Select(user => MapToDTO(user)).ToList();
        }
    }
}
