using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Entity.DTOs;
using Business;
using Entity.Model;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PqrController : ControllerBase
    {
        private readonly PqrBusiness _business;
        private readonly ILogger<PqrController> _logger;

        public PqrController(PqrBusiness business, ILogger<PqrController> logger)
        {
            _business = business;
            _logger = logger;
        }

        // POST: api/Pqr
        [HttpPost]
        [ProducesResponseType(typeof(PqrDto), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] PqrDto dto)
        {
            try
            {
                var created = await _business.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.PqrId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el PQR.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // GET: api/Pqr
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PqrDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pqrs = await _business.GetAllAsync();
                return Ok(pqrs);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los PQRs.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Pqr/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PqrDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pqr = await _business.GetByIdAsync(id);
                if (pqr == null)
                    return NotFound(new { message = "PQR no encontrado" });

                return Ok(pqr);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el PQR con ID {PqrId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/Pqr/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(int id, [FromBody] PqrDto dto)
        {
            try
            {
                if (id != dto.PqrId)
                    return BadRequest(new { message = "El ID del PQR no coincide." });

                var success = await _business.UpdateAsync(dto);
                if (!success)
                    return NotFound(new { message = "PQR no encontrado" });

                return NoContent();
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el PQR con ID {PqrId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/Pqr/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _business.DeleteAsync(id);
                if (!success)
                    return NotFound(new { message = "PQR no encontrado" });

                return NoContent();
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el PQR con ID {PqrId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH: api/Pqr/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(PqrDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdate(int id, [FromBody] JsonPatchDocument<PqrDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "El objeto patch no puede ser nulo" });
            }

            // Validar que solo se quiera modificar ciertos campos
            var allowedPaths = new[] { "/Description", "/Status" };  // Ajusta los campos según tus necesidades

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
                var existingPqr = await _business.GetByIdAsync(id);

                if (existingPqr == null)
                {
                    return NotFound(new { message = "PQR no encontrado" });
                }

                patchDoc.ApplyTo(existingPqr, error =>
                {
                    ModelState.AddModelError(error.Operation.path, error.ErrorMessage);
                });

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedPqr = await _business.UpdateAsync(existingPqr);

                return Ok(updatedPqr);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el PQR con ID {PqrId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE PERMANENTE: api/Pqr/permanent/{id}
        [HttpDelete("permanent/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            try
            {
                var success = await _business.PermanentDeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "PQR no encontrado para eliminar permanentemente" });
                }

                return NoContent();
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el PQR con ID {PqrId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
