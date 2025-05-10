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
    public class UserController : BaseController<User, UserDto>
    {
        private readonly IService<User, UserDto> _userService;

        public UserController(
            IService<User, UserDto> service, 
            ILogger<UserController> logger)
            : base(service, logger)
        {
            _userService = service;
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateUser(int id, [FromBody] JsonPatchDocument<UserDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "El objeto patch no puede ser nulo" });
            }

            // Validate that only allowed paths are modified
            var allowedPaths = new[] { "/Name", "/Email" };
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
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
                }

                patchDoc.ApplyTo(user, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _userService.UpdateAsync(user);
                if (!updated)
                {
                    return StatusCode(500, new { message = "Error al actualizar el usuario" });
                }

                return Ok(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = $"Error al actualizar parcialmente el usuario con ID {id}" });
            }
        }
    }
}