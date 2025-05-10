using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Utilities.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BaseController<TEntity, TDto> : ControllerBase
        where TEntity : class
        where TDto : class
    {
        protected readonly IService<TEntity, TDto> _service;
        protected readonly ILogger<BaseController<TEntity, TDto>> _logger;

        public BaseController(IService<TEntity, TDto> service, ILogger<BaseController<TEntity, TDto>> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(200)] // Eliminado typeof(IEnumerable<TDto>)
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los elementos");
                return StatusCode(500, new { message = "Error al recuperar la lista de elementos" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)] // Eliminado typeof(TDto)
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor que cero" });
            }

            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = $"No se encontró el elemento con ID {id}" });
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el elemento con ID: {Id}", id);
                return StatusCode(500, new { message = $"Error al recuperar el elemento con ID {id}" });
            }
        }

        [HttpPost]
        [ProducesResponseType(201)] // Eliminado typeof(TDto)
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> Create([FromBody] TDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "El objeto no puede ser nulo" });
            }

            try
            {
                var createdItem = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = GetId(createdItem) }, createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el elemento");
                return StatusCode(500, new { message = "Error al crear el elemento" });
            }
        }

        [HttpPut]
        [ProducesResponseType(200)] // Eliminado typeof(TDto)
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> Update([FromBody] TDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "El objeto no puede ser nulo" });
            }

            try
            {
                var result = await _service.UpdateAsync(dto);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el elemento con ID {GetId(dto)}" });
                }

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el elemento");
                return StatusCode(500, new { message = "Error al actualizar el elemento" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor que cero" });
            }

            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el elemento con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el elemento con ID: {Id}", id);
                return StatusCode(500, new { message = $"Error al eliminar el elemento con ID {id}" });
            }
        }

        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> PermanentDelete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID debe ser mayor que cero" });
            }

            try
            {
                var result = await _service.PermanentDeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el elemento con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el elemento con ID: {Id}", id);
                return StatusCode(500, new { message = $"Error al eliminar permanentemente el elemento con ID {id}" });
            }
        }

        // Helper method to get the ID from the DTO
        protected virtual int GetId(TDto dto)
        {
            // Try to get the ID property value
            var idProperty = typeof(TDto).GetProperty("Id");
            if (idProperty != null)
            {
                return (int)idProperty.GetValue(dto);
            }

            // Try to get other common ID names
            var alternativeIdProperties = new[] { "RolId", "FormId", "ModuleId", "UserId", "LoginId", "WorkerId" };
            foreach (var propName in alternativeIdProperties)
            {
                var prop = typeof(TDto).GetProperty(propName);
                if (prop != null)
                {
                    return (int)prop.GetValue(dto);
                }
            }

            throw new InvalidOperationException("No se pudo determinar el ID del DTO");
        }
    }
}