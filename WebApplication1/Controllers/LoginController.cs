using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LoginController : ControllerBase
    {
        private readonly LoginBusiness _loginBusiness;
        private readonly ILogger<LoginController> _logger;

        public LoginController(LoginBusiness loginBusiness, ILogger<LoginController> logger)
        {
            _loginBusiness = loginBusiness ?? throw new ArgumentNullException(nameof(loginBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LoginDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllLogins()
        {
            try
            {
                var logins = await _loginBusiness.GetAllAsync();
                return Ok(logins);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los logins.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LoginDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLoginById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del login debe ser mayor que cero." });
            }

            try
            {
                var login = await _loginBusiness.GetByIdAsync(id);
                if (login == null)
                {
                    return NotFound(new { message = $"No se encontró el login con ID {id}." });
                }

                return Ok(login);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID de login inválido: {LoginId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Login no encontrado con ID: {LoginId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el login con ID: {LoginId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateLogin([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest(new { message = "El objeto login no puede ser nulo." });
            }

            try
            {
                var createdLogin = await _loginBusiness.CreateAsync(loginDto);
                return CreatedAtAction(nameof(GetLoginById), new { id = createdLogin.LoginId }, createdLogin);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear login.");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el login.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(LoginDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateLogin([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || loginDto.LoginId <= 0)
            {
                return BadRequest(new { message = "Datos de login inválidos o ID no proporcionado." });
            }

            try
            {
                var result = await _loginBusiness.UpdateAsync(loginDto);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el login con ID {loginDto.LoginId}." });
                }

                return Ok(loginDto);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el login con ID: {LoginId}", loginDto.LoginId);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el login con ID: {LoginId}", loginDto.LoginId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
[ProducesResponseType(typeof(LoginDto), 200)]
[ProducesResponseType(400)]
[ProducesResponseType(404)]
[ProducesResponseType(500)]
public async Task<IActionResult> PartialUpdateLogin(int id, [FromBody] JsonPatchDocument<LoginDto> patchDoc)
{
    if (patchDoc == null)
    {
        return BadRequest(new { message = "El objeto patch no puede ser nulo." });
    }

    // Validar que solo se puede modificar campos específicos
    var allowedPaths = new[] { "/Username", "/Password" };  // Asumimos que solo se puede actualizar el nombre de usuario y la contraseña

    foreach (var op in patchDoc.Operations)
    {
        // Asegurarse de que el 'path' no tiene espacios adicionales
        var trimmedPath = op.path.Trim();

        // Verificar si la propiedad está permitida
        if (!allowedPaths.Contains(trimmedPath, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = $"Solo se permite modificar los siguientes campos: {string.Join(", ", allowedPaths)}" });
        }
    }

    try
    {
        var existingLogin = await _loginBusiness.GetByIdAsync(id);
        if (existingLogin == null)
        {
            return NotFound(new { message = "Login no encontrado." });
        }

        patchDoc.ApplyTo(existingLogin, error =>
        {
            ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
        });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedLogin = await _loginBusiness.UpdateAsync(existingLogin);
        return Ok(updatedLogin);
    }
    catch (ValidationException ex)
    {
        _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente el login con ID: {LoginId}", id);
        return BadRequest(new { message = ex.Message });
    }
    catch (EntityNotFoundException ex)
    {
        _logger.LogInformation(ex, "Login no encontrado con ID: {LoginId}", id);
        return NotFound(new { message = ex.Message });
    }
    catch (ExternalServiceException ex)
    {
        _logger.LogError(ex, "Error al actualizar parcialmente el login con ID: {LoginId}", id);
        return StatusCode(500, new { message = ex.Message });
    }
}

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogin(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del login debe ser mayor que cero." });
            }

            try
            {
                var result = await _loginBusiness.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el login con ID {id}." });
                }

                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el login con ID: {LoginId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Login no encontrado con ID: {LoginId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el login con ID: {LoginId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
