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
    public class WorkerLoginController : ControllerBase
    {
        private readonly WorkerLoginBusiness _workerLoginBusiness;
        private readonly ILogger<WorkerLoginController> _logger;

        public WorkerLoginController(WorkerLoginBusiness workerLoginBusiness, ILogger<WorkerLoginController> logger)
        {
            _workerLoginBusiness = workerLoginBusiness ?? throw new ArgumentNullException(nameof(workerLoginBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkerLoginDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllWorkerLogins()
        {
            try
            {
                var logins = await _workerLoginBusiness.GetAllAsync();
                return Ok(logins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los inicios de sesión de trabajadores");
                return StatusCode(500, new { message = "Error al recuperar la lista de inicios de sesión de trabajadores" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkerLoginDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetWorkerLoginById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor que cero" });
            }

            try
            {
                var login = await _workerLoginBusiness.GetByIdAsync(id);
                if (login == null)
                {
                    return NotFound(new { message = $"No se encontró el inicio de sesión con ID {id}" });
                }

                return Ok(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el inicio de sesión con ID: {LoginId}", id);
                return StatusCode(500, new { message = $"Error al recuperar el inicio de sesión con ID {id}" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(WorkerLoginDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateWorkerLogin([FromBody] WorkerLoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest(new { message = "El objeto de inicio de sesión no puede ser nulo" });
            }

            try
            {
                var createdLogin = await _workerLoginBusiness.CreateAsync(loginDto);
                return CreatedAtAction(nameof(GetWorkerLoginById), new { id = createdLogin.LoginId }, createdLogin);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("ya existe"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el inicio de sesión de trabajador");
                return StatusCode(500, new { message = "Error al crear el inicio de sesión" });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(WorkerLoginDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateWorkerLogin([FromBody] WorkerLoginDto loginDto)
        {
            if (loginDto == null || loginDto.LoginId <= 0)
            {
                return BadRequest(new { message = "Datos inválidos o ID no proporcionado" });
            }

            try
            {
                var result = await _workerLoginBusiness.UpdateAsync(loginDto);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el inicio de sesión con ID {loginDto.LoginId}" });
                }

                return Ok(loginDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("ya existe"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el inicio de sesión con ID: {LoginId}", loginDto.LoginId);
                return StatusCode(500, new { message = $"Error al actualizar el inicio de sesión con ID {loginDto.LoginId}" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteWorkerLogin(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor que cero" });
            }

            try
            {
                var result = await _workerLoginBusiness.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el inicio de sesión con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el inicio de sesión con ID: {LoginId}", id);
                return StatusCode(500, new { message = $"Error al eliminar el inicio de sesión con ID {id}" });
            }
        }

        // PATCH api/WorkerLogin/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(WorkerLoginDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateWorkerLogin(int id, [FromBody] JsonPatchDocument<WorkerLoginDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "El objeto patch no puede ser nulo" });
            }

            // Validar que solo se quiere modificar los campos permitidos
            var allowedPaths = new[] { "/Username", "/Password", "/Status", "/WorkerId" };

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
                var existingLogin = await _workerLoginBusiness.GetByIdAsync(id);
                if (existingLogin == null)
                {
                    return NotFound(new { message = $"No se encontró el inicio de sesión con ID {id}" });
                }

                patchDoc.ApplyTo(existingLogin, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones
                if (string.IsNullOrWhiteSpace(existingLogin.Username))
                {
                    return BadRequest(new { message = "El nombre de usuario es obligatorio" });
                }

                if (string.IsNullOrWhiteSpace(existingLogin.Password))
                {
                    return BadRequest(new { message = "La contraseña es obligatoria" });
                }

                var result = await _workerLoginBusiness.UpdateAsync(existingLogin);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el inicio de sesión con ID {id}" });
                }

                return Ok(existingLogin);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("ya existe"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el inicio de sesión con ID: {LoginId}", id);
                return StatusCode(500, new { message = $"Error al actualizar parcialmente el inicio de sesión con ID {id}" });
            }
        }

        // DELETE PERMANENTE api/WorkerLogin/permanent/{id}
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PermanentDeleteWorkerLogin(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor que cero" });
            }

            try
            {
                var result = await _workerLoginBusiness.PermanentDeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el inicio de sesión con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el inicio de sesión con ID: {LoginId}", id);
                return StatusCode(500, new { message = $"Error al eliminar permanentemente el inicio de sesión con ID {id}" });
            }
        }
    }
}