using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<FormModuleDto> CreateAsync(FormModuleDto formModuleDto)
    {
        try
        {
            if (formModuleDto.ModuleId <= 0 || formModuleDto.FormId <= 0)
            {
                _logger.LogWarning("ModuleId o FormId no válidos.");
                throw new ArgumentException("El ModuleId o FormId no es válido.");
            }

            var existingAssignment = await _formModuleData.GetByModuleIdAndFormIdAsync(formModuleDto.ModuleId, formModuleDto.FormId);
            if (existingAssignment != null)
            {
                _logger.LogWarning("La asignación de formulario a módulo ya existe.");
                throw new InvalidOperationException("La asignación ya existe.");
            }

            var formModule = MapToEntity(formModuleDto);
            formModule.CreateAt = DateTime.UtcNow;

            var createdFormModule = await _formModuleData.CreateAsync(formModule);

            return MapToDTO(createdFormModule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la asignación de formulario a módulo.");
            throw;
        }
    }

    public async Task<IEnumerable<FormModuleDto>> GetAllAsync()
    {
        try
        {
            var formModules = await _formModuleData.GetAllAsync();
            return MapToDTOList(formModules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las asignaciones de formulario a módulo.");
            throw;
        }
    }

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

            return MapToDTO(formModule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la asignación de formulario a módulo con ID {Id}.", id);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(FormModuleDto formModuleDto)
    {
        try
        {
            if (formModuleDto.ModuleId <= 0 || formModuleDto.FormId <= 0)
            {
                _logger.LogWarning("ModuleId o FormId no válidos.");
                throw new ArgumentException("El ModuleId o FormId no es válido.");
            }

            var formModule = MapToEntity(formModuleDto);
            formModule.DeleteAt = DateTime.MinValue;

            return await _formModuleData.UpdateAsync(formModule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la asignación de formulario a módulo.");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            return await _formModuleData.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la asignación de formulario a módulo.");
            return false;
        }
    }

    public async Task<bool> PermanentDeleteAsync(int id)
    {
        try
        {
            return await _formModuleData.PermanentDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente la asignación de formulario a módulo.");
            return false;
        }
    }

    // ----------------------
    // Métodos de mapeo
    // ----------------------

    private FormModuleDto MapToDTO(FormModule formModule)
    {
        return new FormModuleDto
        {
            Id = formModule.Id,
            ModuleId = formModule.ModuleId,
            FormId = formModule.FormId,
        };
    }

    private FormModule MapToEntity(FormModuleDto formModuleDto)
    {
        return new FormModule
        {
            Id = formModuleDto.Id,
            ModuleId = formModuleDto.ModuleId,
            FormId = formModuleDto.FormId,
        };
    }

    private IEnumerable<FormModuleDto> MapToDTOList(IEnumerable<FormModule> formModules)
    {
        return formModules.Select(MapToDTO).ToList();
    }
}
