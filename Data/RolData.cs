using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;

        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Rol>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID {RolId}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }

        public async Task<Rol> UpdateAsync(Rol rol)
        {
            try
            {
                // Obtener la entidad para verificar si existe
                var rolExistente = await _context.Set<Rol>().FindAsync(rol.Id);
                if (rolExistente == null)
                {
                    return null;
                }

                // Actualizar propiedades
                _context.Entry(rolExistente).CurrentValues.SetValues(rol);
                await _context.SaveChangesAsync();
                
                return rolExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID {RolId}: {ErrorMessage}", rol.Id, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                // Obtener la entidad para verificar si existe
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                {
                    return false;
                }

                // Eliminar la entidad
                _context.Set<Rol>().Remove(rol);
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID {RolId}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }
    }
}