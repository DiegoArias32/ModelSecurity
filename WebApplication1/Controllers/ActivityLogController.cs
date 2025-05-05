using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ActivityLogController : ControllerBase
    {
        private readonly ActivityLogBusiness _activityLogBusiness;
        private readonly ILogger<ActivityLogController> _logger;

        public ActivityLogController(ActivityLogBusiness activityLogBusiness, ILogger<ActivityLogController> logger)
        {
            _activityLogBusiness = activityLogBusiness ?? throw new ArgumentNullException(nameof(activityLogBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST: api/ActivityLog
        [HttpPost]
        [ProducesResponseType(typeof(ActivityLogDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateLog([FromBody] ActivityLogDto logDto)
        {
            try
            {
                if (logDto == null)
                {
                    return BadRequest(new { message = "Los datos del log son requeridos" });
                }

                // Se registrará la fecha actual en el servidor y la IP se capturará automáticamente
                var createdLog = await _activityLogBusiness.LogActivityAsync(
                    logDto.UserId, 
                    logDto.UserName, 
                    logDto.Action, 
                    logDto.EntityType, 
                    logDto.EntityId, 
                    logDto.Details);

                return CreatedAtAction(nameof(GetLogById), new { id = createdLog.Id }, createdLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear registro de actividad");
                return StatusCode(500, new { message = "Error al registrar la actividad" });
            }
        }

        // GET: api/ActivityLog
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ActivityLogDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRecentLogs([FromQuery] int limit = 100, [FromQuery] int offset = 0)
        {
            try
            {
                var logs = await _activityLogBusiness.GetRecentLogsAsync(limit, offset);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad recientes");
                return StatusCode(500, new { message = "Error al obtener registros de actividad" });
            }
        }

        // GET: api/ActivityLog/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ActivityLogDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLogById(int id)
        {
            try
            {
                var log = await _activityLogBusiness.GetLogByIdAsync(id);
                if (log == null)
                {
                    return NotFound(new { message = $"No se encontró el registro de actividad con ID {id}" });
                }

                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registro de actividad con ID {LogId}", id);
                return StatusCode(500, new { message = $"Error al obtener el registro de actividad con ID {id}" });
            }
        }

        // GET: api/ActivityLog/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<ActivityLogDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLogsByUser(string userId, [FromQuery] int limit = 100, [FromQuery] int offset = 0)
        {
            try
            {
                var logs = await _activityLogBusiness.GetLogsByUserAsync(userId, limit, offset);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad del usuario {UserId}", userId);
                return StatusCode(500, new { message = $"Error al obtener registros de actividad del usuario {userId}" });
            }
        }

        // GET: api/ActivityLog/entity/{entityType}
        [HttpGet("entity/{entityType}")]
        [ProducesResponseType(typeof(IEnumerable<ActivityLogDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLogsByEntityType(string entityType, [FromQuery] int limit = 100, [FromQuery] int offset = 0)
        {
            try
            {
                var logs = await _activityLogBusiness.GetLogsByEntityTypeAsync(entityType, limit, offset);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad del tipo de entidad {EntityType}", entityType);
                return StatusCode(500, new { message = $"Error al obtener registros de actividad del tipo de entidad {entityType}" });
            }
        }

        // GET: api/ActivityLog/daterange
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(IEnumerable<ActivityLogDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLogsByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] int limit = 100,
            [FromQuery] int offset = 0)
        {
            try
            {
                if (start > end)
                {
                    return BadRequest(new { message = "La fecha de inicio debe ser menor o igual que la fecha de fin" });
                }

                var logs = await _activityLogBusiness.GetLogsByDateRangeAsync(start, end, limit, offset);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros de actividad por rango de fechas");
                return StatusCode(500, new { message = "Error al obtener registros de actividad por rango de fechas" });
            }
        }
    }
}