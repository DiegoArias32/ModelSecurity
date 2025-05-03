using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{ModuleData}"/> para registrar eventos.</param>
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                module.CreateAt = DateTime.UtcNow; // Establece la fecha actual en UTC
                
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el módulo: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Set<Module>().ToListAsync();
        }

        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener módulo con ID {ModuleId}", id);
                throw;
            }
        }

public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var module = await _context.Set<Module>().FindAsync(id);
        if (module == null)
            return false;

        _context.Set<Module>().Remove(module);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el módulo: {ErrorMessage}", ex.Message);
        return false;
    }
}

        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo: {ErrorMessage}", ex.Message);
                return false;
            }
        }
public async Task<bool> DeleteAsync(int id)
{
    try
    {
        var module = await _context.Set<Module>().FindAsync(id);
        if (module == null)
            return false;

        module.DeleteAt = DateTime.UtcNow; // ✅ Soft delete
        _context.Set<Module>().Update(module);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar el módulo: {ErrorMessage}", ex.Message);
        return false;
    }
}

    }
}
