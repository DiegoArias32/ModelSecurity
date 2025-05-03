using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserBusiness _userBusiness;
        private readonly ILogger<UserController> _logger;

        public UserController(UserBusiness userBusiness, ILogger<UserController> logger)
        {
            _userBusiness = userBusiness ?? throw new ArgumentNullException(nameof(userBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userBusiness.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener usuarios");
                return StatusCode(500, new { message = "Error al recuperar la lista de usuarios" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del usuario debe ser mayor que cero" });
            }

            try
            {
                var user = await _userBusiness.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener el usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = $"Error al recuperar el usuario con ID {id}" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest(new { message = "El objeto usuario no puede ser nulo" });
            }

            try
            {
                var createdUser = await _userBusiness.CreateAsync(userDto);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe un usuario con el email"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear usuario");
                return StatusCode(500, new { message = "Error al crear el usuario" });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            if (userDto == null || userDto.Id <= 0)
            {
                return BadRequest(new { message = "Datos de usuario inválidos o ID no proporcionado" });
            }

            try
            {
                var result = await _userBusiness.UpdateAsync(userDto);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el usuario con ID {userDto.Id}" });
                }

                return Ok(userDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe un usuario con el email"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar el usuario con ID: {UserId}", userDto.Id);
                return StatusCode(500, new { message = $"Error al actualizar el usuario con ID {userDto.Id}" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del usuario debe ser mayor que cero" });
            }

            try
            {
                var result = await _userBusiness.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar el usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = $"Error al eliminar el usuario con ID {id}" });
            }
        }
        
        // PATCH api/User/{id}
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

            // Validar que solo se pueden modificar ciertos campos
            var allowedPaths = new[] { "/Name", "/Email" };

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
                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID del usuario debe ser mayor que cero" });
                }

                var existingUser = await _userBusiness.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
                }

                patchDoc.ApplyTo(existingUser, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones adicionales
                if (string.IsNullOrWhiteSpace(existingUser.Name))
                {
                    return BadRequest(new { message = "El nombre del usuario no puede estar vacío" });
                }

                if (string.IsNullOrWhiteSpace(existingUser.Email))
                {
                    return BadRequest(new { message = "El email del usuario no puede estar vacío" });
                }

                var result = await _userBusiness.UpdateAsync(existingUser);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
                }

                return Ok(existingUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe un usuario con el email"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar parcialmente el usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = $"Error al actualizar parcialmente el usuario con ID {id}" });
            }
        }

        // DELETE PERMANENTE api/User/permanent/{id}
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PermanentDeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del usuario debe ser mayor que cero" });
            }

            try
            {
                var result = await _userBusiness.PermanentDeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el usuario con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar permanentemente el usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = $"Error al eliminar permanentemente el usuario con ID {id}" });
            }
        }
    }
}