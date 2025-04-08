using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class PqrData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PqrData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos y el logger.
        /// </summary>
        public PqrData(ApplicationDbContext context, ILogger<PqrData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo PQR en la base de datos.
        /// </summary>
        public async Task<Pqr> CreateAsync(Pqr pqr)
        {
            try
            {
                await _context.Pqr.AddAsync(pqr);
                await _context.SaveChangesAsync();
                return pqr;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el PQR: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los PQR registrados.
        /// </summary>
        public async Task<IEnumerable<Pqr>> GetAllAsync()
        {
            try
            {
                return await _context.Pqr
                    .Include(p => p.Worker)
                    .Include(p => p.Client)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los PQR: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un PQR por su identificador.
        /// </summary>
        public async Task<Pqr?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Pqr
                    .Include(p => p.Worker)
                    .Include(p => p.Client)
                    .FirstOrDefaultAsync(p => p.PqrId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el PQR con ID {PqrId}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un PQR existente.
        /// </summary>
        public async Task<bool> UpdateAsync(Pqr pqr)
        {
            try
            {
                _context.Pqr.Update(pqr);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el PQR: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un PQR de la base de datos (borrado físico).
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var pqr = await _context.Pqr.FindAsync(id);
                if (pqr == null)
                    return false;

                _context.Pqr.Remove(pqr);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el PQR: {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}
