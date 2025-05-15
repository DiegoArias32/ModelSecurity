using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Entity.Model;
using System;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolFormPermissionController : GenericController<RolFormPermissionDto, RolFormPermission>
    {
        private readonly RolFormPermissionBusiness _rolFormPermissionBusiness;

        public RolFormPermissionController(
            RolFormPermissionBusiness rolFormPermissionBusiness, 
            ILogger<RolFormPermissionController> logger)
            : base(
                rolFormPermissionBusiness,
                logger,
                "permiso de formulario para rol",
                dto => dto.Id,
                new[] { "/RolId", "/FormId", "/PermissionId" }
            )
        {
            _rolFormPermissionBusiness = rolFormPermissionBusiness ?? throw new ArgumentNullException(nameof(rolFormPermissionBusiness));
        }

        // Endpoint específico para obtener por RolId
        [HttpGet("rol/{rolId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByRolId(int rolId)
        {
            if (rolId <= 0)
                return BadRequest(new { message = "El ID del rol debe ser mayor que cero" });

            try
            {
                var permissions = await _rolFormPermissionBusiness.GetRolFormPermissionsByRolIdAsync(rolId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos para el rol con ID {RolId}", rolId);
                return StatusCode(500, new { message = $"Error al obtener permisos para el rol con ID {rolId}" });
            }
        }
    }
}