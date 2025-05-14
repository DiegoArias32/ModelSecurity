using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Business;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class GenericController<TDto, TEntity> : ControllerBase
        where TDto : class
        where TEntity : class
    {
        protected readonly IGenericBusiness<TDto, TEntity> _business;
        protected readonly ILogger _logger;
        protected readonly string _entityName;
        protected readonly Func<TDto, object> _getIdFunc;
        protected readonly string[] _allowedPatchPaths;

        public GenericController(
            IGenericBusiness<TDto, TEntity> business,
            ILogger logger,
            string entityName,
            Func<TDto, object> getIdFunc,
            string[] allowedPatchPaths = null)
        {
            _business = business;
            _logger = logger;
            _entityName = entityName;
            _getIdFunc = getIdFunc;
            _allowedPatchPaths = allowedPatchPaths ?? Array.Empty<string>();
        }

        // GET: api/[controller]
        [HttpGet]
        // No usamos Type genéricos en los atributos
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> GetAll()
        {
            try
            {
                var entities = await _business.GetAllAsync();
                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los {EntityName}", _entityName);
                return StatusCode(500, new { message = $"Error al recuperar la lista de {_entityName}" });
            }
        }

        // GET: api/[controller]/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = $"El ID del {_entityName} debe ser mayor que cero" });
            }

            try
            {
                var entity = await _business.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound(new { message = $"No se encontró el {_entityName} con ID {id}" });
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el {EntityName} con ID: {Id}", _entityName, id);
                return StatusCode(500, new { message = $"Error al recuperar el {_entityName} con ID {id}" });
            }
        }

        // POST: api/[controller]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> Create([FromBody] TDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = $"El objeto {_entityName} no puede ser nulo" });
            }

            try
            {
                var createdEntity = await _business.CreateAsync(dto);
                object id = _getIdFunc(createdEntity);
                return CreatedAtAction(nameof(GetById), new { id }, createdEntity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear {EntityName}", _entityName);
                return StatusCode(500, new { message = $"Error al crear el {_entityName}" });
            }
        }

        // PUT: api/[controller]
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> Update([FromBody] TDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = $"El objeto {_entityName} no puede ser nulo" });
            }

            object id = _getIdFunc(dto);
            if (Convert.ToInt32(id) <= 0)
            {
                return BadRequest(new { message = $"El ID del {_entityName} debe ser mayor que cero" });
            }

            try
            {
                var result = await _business.UpdateAsync(dto);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el {_entityName} con ID {id}" });
                }

                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el {EntityName} con ID: {Id}", _entityName, id);
                return StatusCode(500, new { message = $"Error al actualizar el {_entityName} con ID {id}" });
            }
        }

        // PATCH: api/[controller]/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> PartialUpdate(int id, [FromBody] JsonPatchDocument<TDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "El objeto patch no puede ser nulo" });
            }

            // Validar que solo se pueden modificar campos específicos si se especificaron allowedPatchPaths
            if (_allowedPatchPaths.Length > 0)
            {
                foreach (var op in patchDoc.Operations)
                {
                    var trimmedPath = op.path.Trim();
                    if (!Array.Exists(_allowedPatchPaths, p => p.Equals(trimmedPath, StringComparison.OrdinalIgnoreCase)))
                    {
                        return BadRequest(new { message = $"Solo se permite modificar los siguientes campos: {string.Join(", ", _allowedPatchPaths)}" });
                    }
                }
            }

            try
            {
                var existingEntity = await _business.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return NotFound(new { message = $"No se encontró el {_entityName} con ID {id}" });
                }

                patchDoc.ApplyTo(existingEntity, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _business.UpdateAsync(existingEntity);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el {_entityName} con ID {id}" });
                }

                // Obtenemos la entidad actualizada
                var updatedEntity = await _business.GetByIdAsync(id);
                return Ok(updatedEntity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el {EntityName} con ID: {Id}", _entityName, id);
                return StatusCode(500, new { message = $"Error al actualizar parcialmente el {_entityName} con ID {id}" });
            }
        }

        // DELETE: api/[controller]/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = $"El ID del {_entityName} debe ser mayor que cero" });
            }

            try
            {
                var result = await _business.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el {_entityName} con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el {EntityName} con ID: {Id}", _entityName, id);
                return StatusCode(500, new { message = $"Error al eliminar el {_entityName} con ID {id}" });
            }
        }

        // DELETE: api/[controller]/permanent/{id}
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> PermanentDelete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = $"El ID del {_entityName} debe ser mayor que cero" });
            }

            try
            {
                var result = await _business.PermanentDeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el {_entityName} con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el {EntityName} con ID: {Id}", _entityName, id);
                return StatusCode(500, new { message = $"Error al eliminar permanentemente el {_entityName} con ID {id}" });
            }
        }
    }
}