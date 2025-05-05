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
    public class ActivityLogData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ActivityLogData> _logger;

        public ActivityLogData(ApplicationDbContext context, ILogger<ActivityLogData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ActivityLog> CreateAsync(ActivityLog log)
        {
            try
            {
                // Asegurar que la fecha/hora es UTC
                log.Timestamp = DateTime.UtcNow;
                
                await _context.ActivityLogs.AddAsync(log);
                await _context.SaveChangesAsync();
                
                return log;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear registro de actividad: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetAllAsync(int limit = 100, int offset = 0)
        {
            try
            {
                return await _context.ActivityLogs
                    .OrderByDescending(l => l.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetByUserIdAsync(string userId, int limit = 100, int offset = 0)
        {
            try
            {
                return await _context.ActivityLogs
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad por usuario: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetByEntityTypeAsync(string entityType, int limit = 100, int offset = 0)
        {
            try
            {
                return await _context.ActivityLogs
                    .Where(l => l.EntityType == entityType)
                    .OrderByDescending(l => l.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad por tipo de entidad: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetByDateRangeAsync(DateTime start, DateTime end, int limit = 100, int offset = 0)
        {
            try
            {
                return await _context.ActivityLogs
                    .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                    .OrderByDescending(l => l.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad por rango de fechas: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<ActivityLog> GetByIdAsync(int id)
        {
            try
            {
                return await _context.ActivityLogs
                    .FirstOrDefaultAsync(l => l.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registro de actividad por ID: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}