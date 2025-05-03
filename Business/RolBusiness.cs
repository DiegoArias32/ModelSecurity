using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                return MapToDTOList(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

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

                return MapToDTO(rol);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        public async Task<RolDto> CreateRolAsync(RolDto rolDto)
        {
            try
            {
                ValidateRol(rolDto);

                var rol = MapToEntity(rolDto);
                var rolCreado = await _rolData.CreateAsync(rol);

                return MapToDTO(rolCreado);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", rolDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para actualizar un rol
        public async Task<RolDto> UpdateRolAsync(RolDto rolDto)
        {
            if (rolDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un rol con ID inválido: {RolId}", rolDto.Id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                ValidateRol(rolDto);

                // Verificar si el rol existe
                var rolExistente = await _rolData.GetByIdAsync(rolDto.Id);
                if (rolExistente == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", rolDto.Id);
                    throw new EntityNotFoundException("Rol", rolDto.Id);
                }

                var rol = MapToEntity(rolDto);
                var rolActualizado = await _rolData.UpdateAsync(rol);

                return MapToDTO(rolActualizado);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RolId}", rolDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el rol con ID {rolDto.Id}", ex);
            }
        }

public async Task<bool> PermanentDeleteRolAsync(int id)
{
    if (id <= 0)
    {
        _logger.LogWarning("Se intentó eliminar permanentemente un rol con ID inválido: {RolId}", id);
        throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
    }

    try
    {
        // Verificar si el rol existe
        var rolExistente = await _rolData.GetByIdAsync(id);
        if (rolExistente == null)
        {
            _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
            throw new EntityNotFoundException("Rol", id);
        }

        return await _rolData.PermanentDeleteAsync(id);
    }
    catch (EntityNotFoundException)
    {
        throw;
    }
    catch (ValidationException)
    {
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el rol con ID: {RolId}", id);
        throw new ExternalServiceException("Base de datos", $"Error al eliminar permanentemente el rol con ID {id}", ex);
    }
}


        // Método para eliminar un rol
        public async Task<bool> DeleteRolAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un rol con ID inválido: {RolId}", id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                // Verificar si el rol existe
                var rolExistente = await _rolData.GetByIdAsync(id);
                if (rolExistente == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return await _rolData.DeleteAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el rol con ID {id}", ex);
            }
        }

        // Método para validar los datos del rol
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

            // Más validaciones según sea necesario
        }

        // Método para mapear de Rol a RolDTO
        private RolDto MapToDTO(Rol rol)
        {
            return new RolDto
            {
                Id = rol.Id,
                Name = rol.Name,
                Description = rol.Description,
            };
        }

        // Método para mapear de RolDTO a Rol
        private Rol MapToEntity(RolDto rolDto)
        {
            return new Rol
            {
                Id = rolDto.Id,
                Name = rolDto.Name,
                Description = rolDto.Description
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> roles)
        {
            return roles.Select(rol => MapToDTO(rol)).ToList();
        }
    }
}