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
    public class WorkerLoginData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WorkerLoginData> _logger;

        public WorkerLoginData(ApplicationDbContext context, ILogger<WorkerLoginData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WorkerLogin> CreateAsync(WorkerLogin login)
        {
            try
            {
                // Validar duplicado por username
                var existingLogin = await _context.Set<WorkerLogin>()
                    .FirstOrDefaultAsync(l => l.Username == login.Username);

                if (existingLogin != null)
                {
                    _logger.LogWarning("Intento de crear login duplicado para Worker con username: {Username}", login.Username);
                    throw new InvalidOperationException($"Ya existe un login con el username '{login.Username}'");
                }

                login.CreationDate = DateTime.Now;

                await _context.Set<WorkerLogin>().AddAsync(login);
                await _context.SaveChangesAsync();

                return login;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear el WorkerLogin: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<WorkerLogin>> GetAllAsync()
        {
            try
            {
                return await _context.Set<WorkerLogin>()
                    .Include(w => w.Worker)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener todos los WorkerLogin: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<WorkerLogin?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<WorkerLogin>()
                    .Include(w => w.Worker)
                    .FirstOrDefaultAsync(w => w.id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener WorkerLogin con ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var workerLogin = await _context.Set<WorkerLogin>()
            .FirstOrDefaultAsync(wl => wl.id == id);

        if (workerLogin == null)
            return false;

        _context.Set<WorkerLogin>().Remove(workerLogin);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al eliminar permanentemente el login de trabajador: {Message}", ex.Message);
        return false;
    }
}

        public async Task<WorkerLogin?> GetByLoginIdAsync(int loginId)
        {
            try
            {
                return await _context.Set<WorkerLogin>()
                    .Include(w => w.Worker)
                    .FirstOrDefaultAsync(w => w.LoginId == loginId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener WorkerLogin con LoginId {LoginId}: {Message}", loginId, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(WorkerLogin login)
        {
            try
            {
                // Validar duplicado por username
                var existing = await _context.Set<WorkerLogin>()
                    .FirstOrDefaultAsync(l => l.Username == login.Username && l.id != login.id);

                if (existing != null)
                {
                    _logger.LogWarning("Intento de actualizar WorkerLogin con username duplicado: {Username}", login.Username);
                    throw new InvalidOperationException($"Ya existe un login con el username '{login.Username}'");
                }

                _context.Set<WorkerLogin>().Update(login);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar WorkerLogin: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var login = await _context.Set<WorkerLogin>()
                    .FirstOrDefaultAsync(l => l.id == id);

                if (login == null)
                    return false;

                _context.Set<WorkerLogin>().Remove(login);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar WorkerLogin: {Message}", ex.Message);
                return false;
            }
        }
    }
}