using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Entity.DTOs;
using Business;
using Entity.Model;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAll()
        {
            var pqrs = await _business.GetAllAsync();
            return Ok(pqrs);
        }

        // GET: api/Pqr/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pqr = await _business.GetByIdAsync(id);
            if (pqr == null)
                return NotFound();

            return Ok(pqr);
        }

        // PUT: api/Pqr/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PqrDto dto)
        {
            if (id != dto.PqrId)
                return BadRequest("El ID del PQR no coincide.");

            var success = await _business.UpdateAsync(dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Pqr/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _business.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
