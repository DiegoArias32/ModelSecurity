using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business;
using Entity.DTOs;
using Utilities.Exceptions;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserBusiness _userBusiness;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        /// <param name="userBusiness">Capa de negocio de usuarios</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public UserController(UserBusiness userBusiness, ILogger<UserController> logger)
        {
            _userBusiness = userBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        /// <response code="200">Retorna la lista de usuarios</response>
        /// <response code="500">Error interno del servidor</response>
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
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario solicitado</returns>
        /// <response code="200">Retorna el usuario solicitado</response>
        /// <response code="404">Usuario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userBusiness.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                return Ok(user);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        /// <param name="userDto">Datos del usuario a crear</param>
        /// <returns>Usuario creado</returns>
        /// <response code="201">Retorna el usuario creado</response>
        /// <response code="400">Datos del usuario no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                var createdUser = await _userBusiness.CreateAsync(userDto);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Datos no válidos al crear el usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="userDto">Datos actualizados del usuario</param>
        /// <returns>Resultado de la actualización</returns>
        /// <response code="200">Actualización exitosa</response>
        /// <response code="400">Datos del usuario no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                if (id != userDto.Id)
                {
                    return BadRequest(new { message = "ID del usuario no coincide" });
                }

                var result = await _userBusiness.UpdateAsync(userDto);
                if (result)
                {
                    return Ok();
                }
                return NotFound(new { message = "Usuario no encontrado para actualizar" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Datos no válidos al actualizar el usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>Resultado de la eliminación</returns>
        /// <response code="200">Eliminación exitosa</response>
        /// <response code="404">Usuario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userBusiness.DeleteAsync(id);
                if (result)
                {
                    return Ok();
                }
                return NotFound(new { message = "Usuario no encontrado para eliminar" });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
