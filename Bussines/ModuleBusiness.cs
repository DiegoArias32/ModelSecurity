using Data;
using Entity.Contexts;
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
        private readonly ILogger _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger logger)
        {
            _moduleData = moduleData ?? throw new ArgumentNullException(nameof(moduleData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Obtener todos los módulos
        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                var moduleDtos = new List<ModuleDto>();

                foreach (var module in modules)
                {
                    moduleDtos.Add(new ModuleDto
                    {
                        Id = module.Id,
                        Code = module.Code,
                        Active = module.Active,
                        CreateAt = module.CreateAt
                    });
                }

                return moduleDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos", ex);
            }
        }

        // Obtener un módulo por ID
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

                return new ModuleDto
                {
                    Id = module.Id,
                    Code = module.Code,
                    Active = module.Active,
                    CreateAt = module.CreateAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        // Crear un nuevo módulo
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var module = new Module
                {
                    Code = moduleDto.Code,
                    Active = moduleDto.Active,
                    CreateAt = DateTime.Now // Si necesitas la fecha de creación
                };

                var createdModule = await _moduleData.CreateAsync(module);

                return new ModuleDto
                {
                    Id = createdModule.Id,
                    Code = createdModule.Code,
                    Active = createdModule.Active,
                    CreateAt = createdModule.CreateAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el módulo: {ModuleCode}", moduleDto?.Code ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        // Validar los datos del módulo
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

        // Actualizar un módulo
        public async Task<bool> UpdateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var module = new Module
                {
                    Id = moduleDto.Id,
                    Code = moduleDto.Code,
                    Active = moduleDto.Active,
                    CreateAt = moduleDto.CreateAt // Suponiendo que no cambie
                };

                return await _moduleData.UpdateAsync(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo: {ModuleCode}", moduleDto?.Code ?? "null");
                return false;
            }
        }

        // Eliminar un módulo
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
    }
}
