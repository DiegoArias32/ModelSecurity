using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger<RolBusiness> _logger;

        public RolBusiness(RolData rolData, ILogger<RolBusiness> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }

        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                return roles.Select(rol => new RolDto
                {
                    Id = rol.Id,
                    Name = rol.Name,
                    Active = rol.Active
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // ✅ Agregado: Método para obtener un rol por ID
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _rolData.GetByIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return new RolDto
                {
                    Id = rol.Id,
                    Name = rol.Name,
                    Active = rol.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // ✅ Agregado: Método para crear un rol
        public async Task<RolDto> CreateRolAsync(RolDto rolDto)
        {
            try
            {
                ValidateRol(rolDto);

                var rol = new Rol
                {
                    Name = rolDto.Name,
                    Active = rolDto.Active
                };

                var rolCreado = await _rolData.CreateAsync(rol);

                return new RolDto
                {
                    Id = rolCreado.Id,
                    Name = rolCreado.Name,
                    Active = rolCreado.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", rolDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // ✅ Método para validar los datos del rol
        private void ValidateRol(RolDto rolDto)
        {
            if (rolDto == null)
            {
                throw new ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new ValidationException("Name", "El Name del rol es obligatorio");
            }
        }
    }
}
