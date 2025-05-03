using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class WorkerData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WorkerData> _logger;

        public WorkerData(ApplicationDbContext context, ILogger<WorkerData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo trabajador.
        /// </summary>
        public async Task<Worker> CreateAsync(Worker worker)
        {
            try
            {
                // Validar que no se repita el documento de identidad
                var exists = await _context.Set<Worker>()
                    .FirstOrDefaultAsync(w => w.IdentityDocument == worker.IdentityDocument);
                
                if (exists != null)
                {
                    _logger.LogWarning("Documento de identidad ya registrado: {Identity}", worker.IdentityDocument);
                    throw new InvalidOperationException($"Ya existe un trabajador con el documento '{worker.IdentityDocument}'");
                }

                await _context.Set<Worker>().AddAsync(worker);
                await _context.SaveChangesAsync();
                return worker;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear trabajador: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Retorna todos los trabajadores registrados.
        /// </summary>
        public async Task<IEnumerable<Worker>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Worker>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener trabajadores: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un trabajador por ID.
        /// </summary>
        public async Task<Worker?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Worker>().FirstOrDefaultAsync(w => w.WorkerId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al buscar trabajador con ID {Id}", id);
                throw;
            }
        }

public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var worker = await _context.Set<Worker>()
            .FirstOrDefaultAsync(w => w.WorkerId == id);

        if (worker == null)
            return false;

        _context.Set<Worker>().Remove(worker);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al eliminar permanentemente el trabajador: {Message}", ex.Message);
        return false;
    }
}

        /// <summary>
        /// Actualiza un trabajador existente.
        /// </summary>
        public async Task<bool> UpdateAsync(Worker worker)
        {
            try
            {
                // Validar que no se repita el documento si cambia
                var duplicate = await _context.Set<Worker>()
                    .FirstOrDefaultAsync(w => w.IdentityDocument == worker.IdentityDocument && w.WorkerId != worker.WorkerId);

                if (duplicate != null)
                {
                    _logger.LogWarning("Intento de duplicar documento: {Doc}", worker.IdentityDocument);
                    throw new InvalidOperationException($"Ya existe un trabajador con el documento '{worker.IdentityDocument}'");
                }

                _context.Set<Worker>().Update(worker);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar trabajador: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un trabajador por ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var worker = await _context.Set<Worker>()
                    .FirstOrDefaultAsync(w => w.WorkerId == id);

                if (worker == null)
                    return false;

                _context.Set<Worker>().Remove(worker);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar trabajador: {Message}", ex.Message);
                return false;
            }
        }
    }
}
