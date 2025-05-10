// Data/ActivityLogData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class ActivityLogData : BaseData<ActivityLog>
    {
        public ActivityLogData(IRepository<ActivityLog> repository, ILogger<ActivityLogData> logger)
            : base(repository, logger)
        {
        }

        // Sobrescribir CreateAsync para asegurar que la timestamp sea UTC
        public override async Task<ActivityLog> CreateAsync(ActivityLog log)
        {
            try
            {
                // Asegurar que la fecha/hora es UTC
                log.Timestamp = DateTime.UtcNow;
                
                return await base.CreateAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear registro de actividad: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetByUserIdAsync(string userId, int limit = 100, int offset = 0)
        {
            try
            {
                var allLogs = await base.GetAllAsync();
                return allLogs
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.Timestamp)
                    .Skip(offset)
                    .Take(limit);
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
                var allLogs = await base.GetAllAsync();
                return allLogs
                    .Where(l => l.EntityType == entityType)
                    .OrderByDescending(l => l.Timestamp)
                    .Skip(offset)
                    .Take(limit);
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
                var allLogs = await base.GetAllAsync();
                return allLogs
                    .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                    .OrderByDescending(l => l.Timestamp)
                    .Skip(offset)
                    .Take(limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad por rango de fechas: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}