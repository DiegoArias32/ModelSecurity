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

        
    }
}