using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolController : BaseController<Rol, RolDto>
    {
        private readonly IService<Rol, RolDto> _rolService;

        public RolController(
            IService<Rol, RolDto> service, 
            ILogger<RolController> logger)
            : base(service, logger)
        {
            _rolService = service;
        }

        // Método personalizado para patch
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateRol(int id, [FromBody] JsonPatchDocument<RolDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "El objeto patch no puede ser nulo" });
            }

            // Validate that only allowed paths are modified
            var allowedPaths = new[] { "/Name", "/Description" };
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
                var rol = await _rolService.GetByIdAsync(id);
                if (rol == null)
                {
                    return NotFound(new { message = $"No se encontró el rol con ID {id}" });
                }

                patchDoc.ApplyTo(rol, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _rolService.UpdateAsync(rol);
                if (!updated)
                {
                    return StatusCode(500, new { message = "Error al actualizar el rol" });
                }

                return Ok(rol);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el rol con ID: {RolId}", id);
                return StatusCode(500, new { message = $"Error al actualizar parcialmente el rol con ID {id}" });
            }
        }
    }
}