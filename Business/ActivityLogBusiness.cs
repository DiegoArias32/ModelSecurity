using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Data;
using Entity.Model;
using Entity.DTOs;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Business
{
    public class ActivityLogBusiness
    {
        private readonly ActivityLogData _activityLogData;
        private readonly ILogger<ActivityLogBusiness> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivityLogBusiness(
            ActivityLogData activityLogData, 
            ILogger<ActivityLogBusiness> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _activityLogData = activityLogData ?? throw new ArgumentNullException(nameof(activityLogData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Registra una actividad en el sistema
        /// </summary>
        public async Task<ActivityLogDto> LogActivityAsync(
            string userId,
            string userName,
            string action,
            string entityType,
            string entityId,
            string details = null)
        {
            try
            {
                // Crear entidad para el log
                var log = new ActivityLog
                {
                    UserId = userId,
                    UserName = userName,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Details = details,
                    Timestamp = DateTime.UtcNow
                };

                // Guardar en la base de datos
                var createdLog = await _activityLogData.CreateAsync(log);

                // Devolver como DTO
                return MapToDTO(createdLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar actividad. User: {UserId}, Action: {Action}, Entity: {EntityType}:{EntityId}",
                    userId, action, entityType, entityId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene los logs de actividad más recientes
        /// </summary>
        public async Task<IEnumerable<ActivityLogDto>> GetRecentLogsAsync(int limit = 100, int offset = 0)
        {
            try
            {
                var logs = await _activityLogData.GetAllAsync(limit, offset);
                return logs.Select(log => MapToDTO(log));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs recientes");
                throw;
            }
        }

        /// <summary>
        /// Obtiene los logs de actividad de un usuario específico
        /// </summary>
        public async Task<IEnumerable<ActivityLogDto>> GetLogsByUserAsync(string userId, int limit = 100, int offset = 0)
        {
            try
            {
                var logs = await _activityLogData.GetByUserIdAsync(userId, limit, offset);
                return logs.Select(log => MapToDTO(log));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs del usuario {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene los logs de actividad de un tipo de entidad específico
        /// </summary>
        public async Task<IEnumerable<ActivityLogDto>> GetLogsByEntityTypeAsync(string entityType, int limit = 100, int offset = 0)
        {
            try
            {
                var logs = await _activityLogData.GetByEntityTypeAsync(entityType, limit, offset);
                return logs.Select(log => MapToDTO(log));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs por tipo de entidad {EntityType}", entityType);
                throw;
            }
        }

        /// <summary>
        /// Obtiene los logs de actividad en un rango de fechas
        /// </summary>
        public async Task<IEnumerable<ActivityLogDto>> GetLogsByDateRangeAsync(DateTime start, DateTime end, int limit = 100, int offset = 0)
        {
            try
            {
                var logs = await _activityLogData.GetByDateRangeAsync(start, end, limit, offset);
                return logs.Select(log => MapToDTO(log));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logs por rango de fechas");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un log de actividad específico por su ID
        /// </summary>
        public async Task<ActivityLogDto> GetLogByIdAsync(int id)
        {
            try
            {
                var log = await _activityLogData.GetByIdAsync(id);
                return log != null ? MapToDTO(log) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener log con ID {LogId}", id);
                throw;
            }
        }

        // Método privado para mapear de ActivityLog a ActivityLogDto
        private ActivityLogDto MapToDTO(ActivityLog log)
        {
            return new ActivityLogDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                UserId = log.UserId,
                UserName = log.UserName,
                Action = log.Action,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                Details = log.Details,
            };
        }
    }
}