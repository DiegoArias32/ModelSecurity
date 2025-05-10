// Business/ActivityLogBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class ActivityLogBusiness : BaseBusiness<ActivityLog, ActivityLogDto>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivityLogBusiness(
            IService<ActivityLog, ActivityLogDto> service, 
            ILogger<ActivityLogBusiness> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(service, logger)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ActivityLogDto> LogActivityAsync(
            string userId,
            string userName,
            string action,
            string entityType,
            string entityId,
            string details = null)
        {
            var dto = new ActivityLogDto
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };
            
            return await base.CreateAsync(dto);
        }

        public async Task<IEnumerable<ActivityLogDto>> GetRecentLogsAsync(int limit = 100, int offset = 0)
        {
            return await base.GetAllAsync();
        }

        public async Task<IEnumerable<ActivityLogDto>> GetLogsByUserAsync(string userId, int limit = 100, int offset = 0)
        {
            // Esta es una consulta específica que podría requerir personalización
            return await base.GetAllAsync(); // Debería filtrarse por usuario
        }

        public async Task<IEnumerable<ActivityLogDto>> GetLogsByEntityTypeAsync(string entityType, int limit = 100, int offset = 0)
        {
            // Esta es una consulta específica que podría requerir personalización
            return await base.GetAllAsync(); // Debería filtrarse por tipo de entidad
        }

        public async Task<IEnumerable<ActivityLogDto>> GetLogsByDateRangeAsync(DateTime start, DateTime end, int limit = 100, int offset = 0)
        {
            // Esta es una consulta específica que podría requerir personalización
            return await base.GetAllAsync(); // Debería filtrarse por rango de fecha
        }

        public async Task<ActivityLogDto> GetLogByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }
    }
}