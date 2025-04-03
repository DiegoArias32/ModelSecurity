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

        public RolData(ApplicationDbContext context, ILogger<RolData> logger) // Agregado <RolData>
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
            return await _context.Set<Rol>().ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID {RolId}", id);
                throw;
            }
        }
    }
}
