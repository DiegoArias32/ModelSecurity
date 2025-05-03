using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
{
    try
    {
        return await _context.Set<Permission>()
                             .Where(p => p.DeleteAt == null)
                             .ToListAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener los permisos");
        throw;
    }
}


        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Permission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID {PermissionId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                _context.Set<Permission>().Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var permission = await _context.Set<Permission>().FindAsync(id);
        if (permission == null)
            return false;

        _context.Set<Permission>().Remove(permission);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el permiso: {ErrorMessage}", ex.Message);
        return false;
    }
}

        public async Task<bool> DeleteAsync(int id)
{
    try
    {
        var permission = await _context.Set<Permission>().FindAsync(id);
        if (permission == null)
            return false;

        // Borrado lógico: establecer la fecha actual
        permission.DeleteAt = DateTime.UtcNow;

        _context.Set<Permission>().Update(permission);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar el permiso: {ErrorMessage}", ex.Message);
        return false;
    }
}

    }
}
