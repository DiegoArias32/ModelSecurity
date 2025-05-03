using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolUserController : ControllerBase
    {
        private readonly RolUserBusiness _rolUserBusiness;
        private readonly ILogger<RolUserController> _logger;

        public RolUserController(RolUserBusiness rolUserBusiness, ILogger<RolUserController> logger)
        {
            _rolUserBusiness = rolUserBusiness ?? throw new ArgumentNullException(nameof(rolUserBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolUserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolUsers()
        {
            try
            {
                var rolUsers = await _rolUserBusiness.GetAllAsync();
                return Ok(rolUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las relaciones Rol-User");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolUserById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de relación Rol-User inválido: {RolUserId}", id);
                    return BadRequest(new { message = "El ID debe ser mayor que cero." });
                }

                var rolUser = await _rolUserBusiness.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("Relación Rol-User no encontrada con ID: {RolUserId}", id);
                    return NotFound(new { message = $"No se encontró la relación Rol-User con ID: {id}" });
                }

                return Ok(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación Rol-User con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(RolUserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolUser([FromBody] RolUserDto rolUserDto)
        {
            try
            {
                if (rolUserDto == null)
                {
                    return BadRequest(new { message = "Los datos de la relación Rol-User son requeridos." });
                }

                if (rolUserDto.UserId <= 0 || rolUserDto.RolId <= 0)
                {
                    _logger.LogWarning("Validación fallida al crear relación Rol-User. UserId: {UserId}, RolId: {RolId}", 
                        rolUserDto.UserId, rolUserDto.RolId);
                    return BadRequest(new { message = "El ID de usuario y rol deben ser mayores que cero." });
                }

                var createdRolUser = await _rolUserBusiness.CreateAsync(rolUserDto);
                return CreatedAtAction(nameof(GetRolUserById), new { id = createdRolUser.Id }, createdRolUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear relación Rol-User");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear relación Rol-User");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolUser([FromBody] RolUserDto rolUserDto)
        {
            try
            {
                if (rolUserDto == null)
                {
                    return BadRequest(new { message = "Los datos de la relación Rol-User son requeridos." });
                }

                if (rolUserDto.Id <= 0)
                {
                    _logger.LogWarning("ID de relación Rol-User inválido: {RolUserId}", rolUserDto.Id);
                    return BadRequest(new { message = "El ID debe ser mayor que cero." });
                }

                if (rolUserDto.UserId <= 0 || rolUserDto.RolId <= 0)
                {
                    _logger.LogWarning("Validación fallida al actualizar relación Rol-User. UserId: {UserId}, RolId: {RolId}",
                        rolUserDto.UserId, rolUserDto.RolId);
                    return BadRequest(new { message = "El ID de usuario y rol deben ser mayores que cero." });
                }

                var success = await _rolUserBusiness.UpdateAsync(rolUserDto);
                if (!success)
                {
                    _logger.LogInformation("Relación Rol-User no encontrada con ID: {RolUserId}", rolUserDto.Id);
                    return NotFound(new { message = $"No se encontró la relación Rol-User con ID: {rolUserDto.Id}" });
                }

                // Obtenemos la entidad actualizada para devolverla en la respuesta
                var updatedRolUser = await _rolUserBusiness.GetByIdAsync(rolUserDto.Id);
                return Ok(updatedRolUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar relación Rol-User");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar relación Rol-User");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de relación Rol-User inválido: {RolUserId}", id);
                    return BadRequest(new { message = "El ID debe ser mayor que cero." });
                }

                var success = await _rolUserBusiness.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogInformation("Relación Rol-User no encontrada con ID: {RolUserId}", id);
                    return NotFound(new { message = $"No se encontró la relación Rol-User con ID: {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar relación Rol-User con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH api/RolUser/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateRolUser(int id, [FromBody] JsonPatchDocument<RolUserDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "El objeto patch no puede ser nulo" });
            }

            // Validar campos permitidos para modificar
            var allowedPaths = new[] { "/UserId", "/RolId" };

            foreach (var op in patchDoc.Operations)
            {
                // Asegúrate de que el 'path' no tiene espacios adicionales
                var trimmedPath = op.path.Trim();

                // Verificamos si la propiedad está permitida (ignorando mayúsculas/minúsculas)
                if (!allowedPaths.Contains(trimmedPath, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = $"Solo se permite modificar los siguientes campos: {string.Join(", ", allowedPaths)}" });
                }
            }

            try
            {
                var existingRolUser = await _rolUserBusiness.GetByIdAsync(id);
                if (existingRolUser == null)
                {
                    return NotFound(new { message = $"No se encontró la relación Rol-User con ID: {id}" });
                }

                patchDoc.ApplyTo(existingRolUser, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones adicionales
                if (existingRolUser.UserId <= 0 || existingRolUser.RolId <= 0)
                {
                    return BadRequest(new { message = "El ID de usuario y rol deben ser mayores que cero." });
                }

                var success = await _rolUserBusiness.UpdateAsync(existingRolUser);
                if (!success)
                {
                    return NotFound(new { message = $"No se encontró la relación Rol-User con ID: {id}" });
                }

                // Obtener la entidad actualizada
                var updatedRolUser = await _rolUserBusiness.GetByIdAsync(id);
                return Ok(updatedRolUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la relación Rol-User");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE PERMANENTE api/RolUser/permanent/{id}
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PermanentDeleteRolUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de relación Rol-User inválido: {RolUserId}", id);
                    return BadRequest(new { message = "El ID debe ser mayor que cero." });
                }

                var deleted = await _rolUserBusiness.PermanentDeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"No se encontró la relación Rol-User con ID: {id} para eliminar permanentemente" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente la relación Rol-User con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}