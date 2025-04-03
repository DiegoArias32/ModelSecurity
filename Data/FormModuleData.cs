using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class FormModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger; // ✅ Uso correcto de ILogger<T>

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                formModule.CreateAt = DateTime.UtcNow;
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

        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _context.Set<FormModule>()
                .Include(fm => fm.Module)
                .Include(fm => fm.Form)
                .ToListAsync();
        }

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

        public async Task<FormModule?> GetByModuleIdAndFormIdAsync(int moduleId, int formId)
        {
            try
            {
                return await _context.Set<FormModule>()
                    .Where(fm => fm.ModuleId == moduleId && fm.FormId == formId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la asignación de formulario a módulo con ModuleId {ModuleId} y FormId {FormId}", moduleId, formId);
                throw;
            }
        }

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
}
