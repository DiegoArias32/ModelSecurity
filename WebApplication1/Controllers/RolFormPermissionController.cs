using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolFormPermissionController : ControllerBase
    {
        private readonly RolFormPermissionBusiness _rolFormPermissionBusiness;
        private readonly ILogger<RolFormPermissionController> _logger;

        public RolFormPermissionController(RolFormPermissionBusiness rolFormPermissionBusiness, ILogger<RolFormPermissionController> logger)
        {
            _rolFormPermissionBusiness = rolFormPermissionBusiness ?? throw new ArgumentNullException(nameof(rolFormPermissionBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolFormPermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolFormPermissions()
        {
            try
            {
                var permissions = await _rolFormPermissionBusiness.GetAllRolFormPermissionsAsync();
                return Ok(permissions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos de formularios para roles");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormPermissionById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de permiso de formulario para rol inválido: {PermissionId}", id);
                    return BadRequest(new { message = "El ID debe ser mayor que cero." });
                }

                var permission = await _rolFormPermissionBusiness.GetRolFormPermissionByIdAsync(id);
                return Ok(permission);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso de formulario para rol no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso de formulario para rol con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("rol/{rolId}")]
        [ProducesResponseType(typeof(IEnumerable<RolFormPermissionDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormPermissionsByRolId(int rolId)
        {
            try
            {
                if (rolId <= 0)
                {
                    _logger.LogWarning("ID de rol inválido: {RolId}", rolId);
                    return BadRequest(new { message = "El ID del rol debe ser mayor que cero." });
                }

                var permissions = await _rolFormPermissionBusiness.GetRolFormPermissionsByRolIdAsync(rolId);
                return Ok(permissions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos de formularios para rol con ID: {RolId}", rolId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(RolFormPermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolFormPermission([FromBody] RolFormPermissionDto permissionDto)
        {
            try
            {
                if (permissionDto == null)
                {
                    return BadRequest(new { message = "Los datos del permiso son requeridos." });
                }

                if (permissionDto.RolId <= 0 || permissionDto.FormId <= 0)
                {
                    _logger.LogWarning("Validación fallida al crear permiso de formulario para rol. RolId: {RolId}, FormId: {FormId}", 
                        permissionDto.RolId, permissionDto.FormId);
                    return BadRequest(new { message = "El ID de rol y formulario deben ser mayores que cero." });
                }

                var createdPermission = await _rolFormPermissionBusiness.CreateRolFormPermissionAsync(permissionDto);
                return CreatedAtAction(nameof(GetRolFormPermissionById), new { id = createdPermission.Id }, createdPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear permiso de formulario para rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear permiso de formulario para rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(RolFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolFormPermission([FromBody] RolFormPermissionDto permissionDto)
        {
            try
            {
                if (permissionDto == null)
                {
                    return BadRequest(new { message = "Los datos del permiso son requeridos." });
                }

                if (permissionDto.Id <= 0)
                {
                    _logger.LogWarning("ID de permiso de formulario para rol inválido: {PermissionId}", permissionDto.Id);
                    return BadRequest(new { message = "El ID debe ser mayor que cero." });
                }

                if (permissionDto.RolId <= 0 || permissionDto.FormId <= 0)
                {
                    _logger.LogWarning("Validación fallida al actualizar permiso de formulario para rol. RolId: {RolId}, FormId: {FormId}",
                        permissionDto.RolId, permissionDto.FormId);
                    return BadRequest(new { message = "El ID de rol y formulario deben ser mayores que cero." });
                }

                var success = await _rolFormPermissionBusiness.UpdateRolFormPermissionAsync(permissionDto);
                if (!success)
                {
                    return NotFound(new { message = $"No se encontró el permiso de formulario para rol con ID: {permissionDto.Id}" });
                }

                // Obtenemos el objeto actualizado
                var updatedPermission = await _rolFormPermissionBusiness.GetRolFormPermissionByIdAsync(permissionDto.Id);
                return Ok(updatedPermission);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso de formulario para rol no encontrado con ID: {PermissionId}", permissionDto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar permiso de formulario para rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolFormPermission(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de permiso de formulario para rol inválido: {PermissionId}", id);
                    return BadRequest(new { message = "El ID debe ser mayor que cero." });
                }

                var success = await _rolFormPermissionBusiness.DeleteRolFormPermissionAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"No se encontró el permiso de formulario para rol con ID: {id}" });
                }

                return NoContent();
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar permiso de formulario para rol con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}