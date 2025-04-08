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
    public class WorkerController : ControllerBase
    {
        private readonly WorkerBusiness _workerBusiness;
        private readonly ILogger<WorkerController> _logger;

        public WorkerController(WorkerBusiness workerBusiness, ILogger<WorkerController> logger)
        {
            _workerBusiness = workerBusiness ?? throw new ArgumentNullException(nameof(workerBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkerDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllWorkers()
        {
            try
            {
                var workers = await _workerBusiness.GetAllAsync();
                return Ok(workers);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener trabajadores");
                return StatusCode(500, new { message = "Error al recuperar la lista de trabajadores" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkerDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetWorkerById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del trabajador debe ser mayor que cero" });
            }

            try
            {
                var worker = await _workerBusiness.GetByIdAsync(id);
                if (worker == null)
                {
                    return NotFound(new { message = $"No se encontró el trabajador con ID {id}" });
                }

                return Ok(worker);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener el trabajador con ID: {WorkerId}", id);
                return StatusCode(500, new { message = $"Error al recuperar el trabajador con ID {id}" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(WorkerDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateWorker([FromBody] WorkerDto workerDto)
        {
            if (workerDto == null)
            {
                return BadRequest(new { message = "El objeto trabajador no puede ser nulo" });
            }

            try
            {
                var createdWorker = await _workerBusiness.CreateAsync(workerDto);
                return CreatedAtAction(nameof(GetWorkerById), new { id = createdWorker.WorkerId }, createdWorker);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear trabajador");
                return StatusCode(500, new { message = "Error al crear el trabajador" });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(WorkerDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateWorker([FromBody] WorkerDto workerDto)
        {
            if (workerDto == null || workerDto.WorkerId <= 0)
            {
                return BadRequest(new { message = "Datos del trabajador inválidos o ID no proporcionado" });
            }

            try
            {
                var result = await _workerBusiness.UpdateAsync(workerDto);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el trabajador con ID {workerDto.WorkerId}" });
                }

                return Ok(workerDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar el trabajador con ID: {WorkerId}", workerDto.WorkerId);
                return StatusCode(500, new { message = $"Error al actualizar el trabajador con ID {workerDto.WorkerId}" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID del trabajador debe ser mayor que cero" });
            }

            try
            {
                var result = await _workerBusiness.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el trabajador con ID {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar el trabajador con ID: {WorkerId}", id);
                return StatusCode(500, new { message = $"Error al eliminar el trabajador con ID {id}" });
            }
        }
        
    }
}
