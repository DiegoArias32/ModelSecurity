using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using Utilities.Exceptions;
using System;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionBusiness _permissionBusiness;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(PermissionBusiness permissionBusiness, ILogger<PermissionController> logger)
        {
            _permissionBusiness = permissionBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissions = await _permissionBusiness.GetAllAsync();
                return Ok(permissions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var permission = await _permissionBusiness.GetByIdAsync(id);
                if (permission == null)
                    return NotFound(new { message = "Permiso no encontrado" });

                return Ok(permission);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDto permissionDto)
        {
            try
            {
                var createdPermission = await _permissionBusiness.CreateAsync(permissionDto);
                return CreatedAtAction(nameof(GetPermissionById), new { id = createdPermission.Id }, createdPermission);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Datos inválidos para el permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] PermissionDto permissionDto)
        {
            try
            {
                if (id != permissionDto.Id)
                    return BadRequest(new { message = "ID del permiso no coincide" });

                var updated = await _permissionBusiness.UpdateAsync(permissionDto);
                return updated ? Ok() : NotFound(new { message = "Permiso no encontrado" });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var deleted = await _permissionBusiness.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Permiso no encontrado" });
                }

                return NoContent();
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH api/Permission/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdatePermission(int id, [FromBody] JsonPatchDocument<PermissionDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "El objeto patch no puede ser nulo" });
            }

            // Validar que solo se quiere modificar campos específicos
            var allowedPaths = new[] { "/Name", "/Description" };  // Ajusta los campos según tus necesidades

            foreach (var op in patchDoc.Operations)
            {
                var trimmedPath = op.path.Trim();

                if (!allowedPaths.Contains(trimmedPath, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = $"Solo se permite modificar los siguientes campos: {string.Join(", ", allowedPaths)}" });
                }
            }

            try
            {
                var existingPermission = await _permissionBusiness.GetByIdAsync(id);

                if (existingPermission == null)
                {
                    return NotFound(new { message = "Permiso no encontrado" });
                }

                patchDoc.ApplyTo(existingPermission, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedPermission = await _permissionBusiness.UpdateAsync(existingPermission);

                return Ok(updatedPermission);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE PERMANENTE api/Permission/permanent/{id}
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PermanentDeletePermission(int id)
        {
            try
            {
                var deleted = await _permissionBusiness.PermanentDeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Permiso no encontrado para eliminar permanentemente" });
                }

                return NoContent();
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
