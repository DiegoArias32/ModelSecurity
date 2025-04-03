using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Data;

public class FormModuleBusiness
{
    private readonly FormModuleData _formModuleData;
    private readonly ILogger<FormModuleBusiness> _logger;

    public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormModuleBusiness> logger)
    {
        _formModuleData = formModuleData ?? throw new ArgumentNullException(nameof(formModuleData));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea una nueva asignación de formulario a módulo.
    /// </summary>
    /// <param name="formModuleDto">DTO con los datos para la asignación.</param>
    /// <returns>El DTO de la asignación creada.</returns>
    public async Task<FormModuleDto> CreateAsync(FormModuleDto formModuleDto)
    {
        try
        {
            // Validar que los datos sean correctos
            if (formModuleDto.ModuleId <= 0 || formModuleDto.FormId <= 0)
            {
                _logger.LogWarning("ModuleId o FormId no válidos.");
                throw new ArgumentException("El ModuleId o FormId no es válido.");
            }

            // Verificar si ya existe la asignación en la base de datos
            var existingAssignment = await _formModuleData.GetByModuleIdAndFormIdAsync(formModuleDto.ModuleId, formModuleDto.FormId);
            if (existingAssignment != null)
            {
                _logger.LogWarning("La asignación de formulario a módulo ya existe.");
                throw new InvalidOperationException("La asignación ya existe.");
            }

            // Convertimos el DTO en la entidad FormModule
            var formModule = new FormModule
            {
                ModuleId = formModuleDto.ModuleId,
                FormId = formModuleDto.FormId,
                CreateAt = DateTime.UtcNow,
                DeleteAt = DateTime.MinValue // No eliminado
            };

            // Crear la asignación en la base de datos
            var createdFormModule = await _formModuleData.CreateAsync(formModule);

            // Convertir la entidad FormModule a DTO para devolverlo
            var resultDto = new FormModuleDto
            {
                Id = createdFormModule.Id,
                ModuleId = createdFormModule.ModuleId,
                FormId = createdFormModule.FormId,
                CreateAt = createdFormModule.CreateAt
            };

            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la asignación de formulario a módulo.");
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de formularios a módulos.
    /// </summary>
    /// <returns>Lista de DTOs de las asignaciones.</returns>
    public async Task<IEnumerable<FormModuleDto>> GetAllAsync()
    {
        try
        {
            var formModules = await _formModuleData.GetAllAsync();

            // Convertir las entidades FormModule a DTOs
            var formModuleDtos = new List<FormModuleDto>();
            foreach (var formModule in formModules)
            {
                formModuleDtos.Add(new FormModuleDto
                {
                    Id = formModule.Id,
                    ModuleId = formModule.ModuleId,
                    FormId = formModule.FormId,
                    CreateAt = formModule.CreateAt
                });
            }

            return formModuleDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las asignaciones de formulario a módulo.");
            throw;
        }
    }

    /// <summary>
    /// Obtiene una asignación de formulario a módulo por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la asignación.</param>
    /// <returns>DTO de la asignación de formulario a módulo.</returns>
    public async Task<FormModuleDto> GetByIdAsync(int id)
    {
        try
        {
            var formModule = await _formModuleData.GetByIdAsync(id);

            if (formModule == null)
            {
                _logger.LogWarning("Asignación de formulario a módulo con ID {Id} no encontrada.", id);
                return null;
            }

            // Convertir la entidad FormModule a DTO
            var formModuleDto = new FormModuleDto
            {
                Id = formModule.Id,
                ModuleId = formModule.ModuleId,
                FormId = formModule.FormId,
                CreateAt = formModule.CreateAt
            };

            return formModuleDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la asignación de formulario a módulo con ID {Id}.", id);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una asignación de formulario a módulo existente.
    /// </summary>
    /// <param name="formModuleDto">DTO con los datos actualizados.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateAsync(FormModuleDto formModuleDto)
    {
        try
        {
            // Validar que los datos sean correctos
            if (formModuleDto.ModuleId <= 0 || formModuleDto.FormId <= 0)
            {
                _logger.LogWarning("ModuleId o FormId no válidos.");
                throw new ArgumentException("El ModuleId o FormId no es válido.");
            }

            // Convertir el DTO en la entidad FormModule
            var formModule = new FormModule
            {
                Id = formModuleDto.Id,
                ModuleId = formModuleDto.ModuleId,
                FormId = formModuleDto.FormId,
                CreateAt = formModuleDto.CreateAt, // No se debe modificar
                DeleteAt = DateTime.MinValue // No eliminado
            };

            // Actualizar la asignación en la base de datos
            var result = await _formModuleData.UpdateAsync(formModule);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la asignación de formulario a módulo.");
            return false;
        }
    }

    /// <summary>
    /// Elimina una asignación de formulario a módulo.
    /// </summary>
    /// <param name="id">Identificador de la asignación a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var result = await _formModuleData.DeleteAsync(id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la asignación de formulario a módulo.");
            return false;
        }
    }

    /// <summary>
    /// Elimina permanentemente una asignación de formulario a módulo.
    /// </summary>
    /// <param name="id">Identificador de la asignación a eliminar permanentemente.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> PermanentDeleteAsync(int id)
    {
        try
        {
            var result = await _formModuleData.PermanentDeleteAsync(id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente la asignación de formulario a módulo.");
            return false;
        }
    }
}
