using Business;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class WorkerLoginController : GenericController<WorkerLoginDto, WorkerLogin>
    {
        private readonly WorkerLoginBusiness _workerLoginBusiness;

        public WorkerLoginController(WorkerLoginBusiness workerLoginBusiness, ILogger<WorkerLoginController> logger)
            : base(
                workerLoginBusiness,
                logger,
                "login de trabajador",
                dto => dto.id, // Nota: Usamos id (minúscula) en lugar de Id
                new[] { "/Username", "/Password", "/Status", "/WorkerId", "/LoginId" }
            )
        {
            _workerLoginBusiness = workerLoginBusiness ?? throw new ArgumentNullException(nameof(workerLoginBusiness));
        }

        // Endpoint adicional para búsqueda por LoginId
        [HttpGet("byloginid/{loginId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByLoginId(int loginId)
        {
            if (loginId <= 0)
                return BadRequest(new { message = "El LoginId debe ser mayor que cero" });

            try
            {
                var login = await _workerLoginBusiness.GetByLoginIdAsync(loginId);
                if (login == null)
                    return NotFound(new { message = $"No se encontró el login con LoginId {loginId}" });

                return Ok(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener login por LoginId {LoginId}", loginId);
                return StatusCode(500, new { message = $"Error al obtener el login con LoginId {loginId}" });
            }
        }
    }
}