using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleBusiness> _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
        {
            _moduleData = moduleData ?? throw new ArgumentNullException(nameof(moduleData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                return MapToDTOList(modules); // Usamos MapToDTOList para convertir la lista de módulos
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos", ex);
            }
        }

        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo con ID inválido: {ModuleId}", id);
                throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Módulo", id);
                }

                return MapToDTO(module); // Usamos MapToDTO aquí para convertir el módulo a DTO
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var module = MapToEntity(moduleDto); // Usamos MapToEntity para convertir el DTO a la entidad

                var createdModule = await _moduleData.CreateAsync(module);

                return MapToDTO(createdModule); // Usamos MapToDTO para convertir la entidad creada a DTO
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el módulo: {ModuleCode}", moduleDto?.Code ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto módulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Code))
            {
                _logger.LogWarning("Se intentó crear un módulo con Code vacío");
                throw new ValidationException("Code", "El Code del módulo es obligatorio");
            }
        }

        public async Task<bool> PermanentDeleteModuleAsync(int id)
{
    try
    {
        if (id <= 0)
        {
            _logger.LogWarning("Se intentó eliminar permanentemente un módulo con ID inválido: {ModuleId}", id);
            throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
        }

        // Verificar que exista el módulo
        var module = await _moduleData.GetByIdAsync(id);
        if (module == null)
        {
            _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
            throw new EntityNotFoundException("Módulo", id);
        }

        return await _moduleData.PermanentDeleteAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el módulo con ID: {ModuleId}", id);
        return false;
    }
}

        public async Task<bool> UpdateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var module = MapToEntity(moduleDto); // Convertimos el DTO a la entidad para actualizar

                return await _moduleData.UpdateAsync(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo: {ModuleCode}", moduleDto?.Code ?? "null");
                return false;
            }
        }

        public async Task<bool> DeleteModuleAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Se intentó eliminar un módulo con ID inválido: {ModuleId}", id);
                    throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
                }

                return await _moduleData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo con ID: {ModuleId}", id);
                return false;
            }
        }

        // Método para mapear de Module a ModuleDto
        private ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Code = module.Code,
                Active = module.Active,
            };
        }

        // Método para mapear de ModuleDto a Module
        private Module MapToEntity(ModuleDto moduleDto)
        {
            return new Module
            {
                Id = moduleDto.Id,
                Code = moduleDto.Code,
                Active = moduleDto.Active,
};
        }

        // Método para mapear una lista de Module a una lista de ModuleDto
        private IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> modules)
        {
            return modules.Select(module => MapToDTO(module)).ToList();
        }
    }
}
