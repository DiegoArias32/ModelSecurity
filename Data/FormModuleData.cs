using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

public class FormModuleData
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public FormModuleData(ApplicationDbContext context, ILogger logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea una nueva asignación de formulario a módulo.
    /// </summary>
    /// <param name="formModule">Instancia de FormModule a crear.</param>
    /// <returns>La asignación de formulario creada.</returns>
    public async Task<FormModule> CreateAsync(FormModule formModule)
    {
        try
        {
            formModule.CreatedAt = DateTime.UtcNow;
            await _context.Set<FormModule>().AddAsync(formModule);
            await _context.SaveChangesAsync();
            return formModule;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la asignación de formulario a módulo: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de formularios a módulos.
    /// </summary>
    /// <returns>Lista de asignaciones de formularios a módulos.</returns>
    public async Task<IEnumerable<FormModule>> GetAllAsync()
    {
        return await _context.Set<FormModule>()
            .Include(fm => fm.Module)
            .Include(fm => fm.Form)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una asignación de formulario a módulo por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la asignación.</param>
    /// <returns>La asignación de formulario a módulo encontrada.</returns>
    public async Task<FormModule?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Set<FormModule>()
                .Include(fm => fm.Module)
                .Include(fm => fm.Form)
                .FirstOrDefaultAsync(fm => fm.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asignación de formulario a módulo con ID {FormModuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de formularios para un módulo específico.
    /// </summary>
    /// <param name="moduleId">Identificador del módulo.</param>
    /// <returns>Lista de asignaciones de formularios del módulo.</returns>
    public async Task<IEnumerable<FormModule>> GetByModuleIdAsync(int moduleId)
    {
        try
        {
            return await _context.Set<FormModule>()
                .Where(fm => fm.ModuleId == moduleId)
                .Include(fm => fm.Module)
                .Include(fm => fm.Form)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener formularios para módulo con ID {ModuleId}", moduleId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de módulos para un formulario específico.
    /// </summary>
    /// <param name="formId">Identificador del formulario.</param>
    /// <returns>Lista de asignaciones de módulos del formulario.</returns>
    public async Task<IEnumerable<FormModule>> GetByFormIdAsync(int formId)
    {
        try
        {
            return await _context.Set<FormModule>()
                .Where(fm => fm.FormId == formId)
                .Include(fm => fm.Module)
                .Include(fm => fm.Form)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener módulos para formulario con ID {FormId}", formId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una asignación de formulario a módulo existente.
    /// </summary>
    /// <param name="formModule">Objeto con la información actualizada.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateAsync(FormModule formModule)
    {
        try
        {
            _context.Set<FormModule>().Update(formModule);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la asignación de formulario a módulo: {ErrorMessage}", ex.Message);
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
            var formModule = await _context.Set<FormModule>().FindAsync(id);
            if (formModule == null)
                return false;

            formModule.DeleteAt = DateTime.UtcNow;
            _context.Set<FormModule>().Update(formModule);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la asignación de formulario a módulo: {ErrorMessage}", ex.Message);
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
            var formModule = await _context.Set<FormModule>().FindAsync(id);
            if (formModule == null)
                return false;

            _context.Set<FormModule>().Remove(formModule);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente la asignación de formulario a módulo: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}