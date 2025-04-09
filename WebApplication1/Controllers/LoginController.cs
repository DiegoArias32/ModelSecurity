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
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener logins");
                return StatusCode(500, new { message = "Error al recuperar la lista de logins" });
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
                return BadRequest(new { message = "El ID del login debe ser mayor que cero" });
            }

            try
            {
                var login = await _loginBusiness.GetByIdAsync(id);
                if (login == null)
                {
                    return NotFound(new { message = $"No se encontró el login con ID {id}" });
                }

                return Ok(login);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener el login con ID: {LoginId}", id);
                return StatusCode(500, new { message = $"Error al recuperar el login con ID {id}" });
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
                return BadRequest(new { message = "El objeto login no puede ser nulo" });
            }

            try
            {
                var createdLogin = await _loginBusiness.CreateAsync(loginDto);
                return CreatedAtAction(nameof(GetLoginById), new { id = createdLogin.LoginId }, createdLogin);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear login");
                return StatusCode(500, new { message = "Error al crear el login" });
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
                return BadRequest(new { message = "Datos de login inválidos o ID no proporcionado" });
            }

            try
            {
                var result = await _loginBusiness.UpdateAsync(loginDto);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el login con ID {loginDto.LoginId}" });
                }

                return Ok(loginDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar el login con ID: {LoginId}", loginDto.LoginId);
                return StatusCode(500, new { message = $"Error al actualizar el login con ID {loginDto.LoginId}" });
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
                return BadRequest(new { message = "El ID del login debe ser mayor que cero" });
            }

            try
            {
                var result = await _loginBusiness.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el login con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar el login con ID: {LoginId}", id);
                return StatusCode(500, new { message = $"Error al eliminar el login con ID {id}" });
            }
        }
    }
}
