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
    }
}
